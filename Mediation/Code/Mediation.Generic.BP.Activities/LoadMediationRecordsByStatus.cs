﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Mediation.Generic.Business;
using Mediation.Generic.Data;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Entities;

namespace Mediation.Generic.BP.Activities
{
    public class LoadMediationRecordsByStatusInput
    {
        public EventStatus EventStatus { get; set; }
        public Guid DataRecordTypeId { get; set; }
        public Guid MediationDefinitionId { get; set; }
        public BaseQueue<SessionIdsBatch> SessionIds { get; set; }
    }
    public sealed class LoadMediationRecordsByStatus : DependentAsyncActivity<LoadMediationRecordsByStatusInput>
    {
        [RequiredArgument]
        public InArgument<EventStatus> EventStatus { get; set; }

        [RequiredArgument]
        public InArgument<Guid> DataRecordTypeId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> MediationDefinitionId { get; set; }

        [RequiredArgument]
        public OutArgument<BaseQueue<SessionIdsBatch>> SessionIds { get; set; }

        protected override void DoWork(LoadMediationRecordsByStatusInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Start Loading Mediation Records By Status Activity");
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
            dataManager.DataRecordTypeId = inputArgument.DataRecordTypeId;
            MediationRecordsManager manager = new MediationRecordsManager();
            SessionIdsBatch batch = new SessionIdsBatch();
            manager.GetMediationRecordsByStatus(inputArgument.MediationDefinitionId, inputArgument.EventStatus, inputArgument.DataRecordTypeId, (sessionIdLoaded) =>
            {
                batch.SessionIds.Add(sessionIdLoaded);
                if (batch.SessionIds.Count >= 50000)
                {
                    inputArgument.SessionIds.Enqueue(batch);
                    batch = new SessionIdsBatch();
                }
            });

            if (batch.SessionIds.Count > 0)
            {
                inputArgument.SessionIds.Enqueue(batch);
            }
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "End Loading Mediation Records By Status Activity");

        }

        protected override LoadMediationRecordsByStatusInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new LoadMediationRecordsByStatusInput
            {
                DataRecordTypeId = this.DataRecordTypeId.Get(context),
                MediationDefinitionId = this.MediationDefinitionId.Get(context),
                SessionIds = this.SessionIds.Get(context),
                EventStatus = this.EventStatus.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.SessionIds.Get(context) == null)
                this.SessionIds.Set(context, new MemoryQueue<SessionIdsBatch>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
