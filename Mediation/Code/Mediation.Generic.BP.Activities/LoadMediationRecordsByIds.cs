using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Mediation.Generic.Business;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Common;

namespace Mediation.Generic.BP.Activities
{
    public class LoadMediationRecordsByIdsInput
    {
        public IEnumerable<string> EventIds { get; set; }
        public BaseQueue<MediationRecordBatch> MediationRecords { get; set; }
        public MediationDefinition MediationDefinition { get; set; }
    }


    public sealed class LoadMediationRecordsByIds : DependentAsyncActivity<LoadMediationRecordsByIdsInput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<string>> EventIds { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<MediationRecordBatch>> MediationRecords { get; set; }

        [RequiredArgument]
        public InArgument<MediationDefinition> MediationDefinition { get; set; }

        protected override LoadMediationRecordsByIdsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new LoadMediationRecordsByIdsInput
            {
                EventIds = this.EventIds.Get(context),
                MediationRecords = this.MediationRecords.Get(context),
                MediationDefinition = this.MediationDefinition.Get(context)
            };
        }

        protected override void DoWork(LoadMediationRecordsByIdsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            MediationRecordsManager manager = new MediationRecordsManager();

            var mediationRecords = manager.GetMediationRecordsByIds(inputArgument.MediationDefinition.MediationDefinitionId, inputArgument.EventIds, inputArgument.MediationDefinition.ParsedRecordTypeId);
            Dictionary<string, SessionRecords> recordsBySessionId = new Dictionary<string, SessionRecords>();
            foreach (var stagingRecord in mediationRecords)
            {
                SessionRecords sessionRecords = recordsBySessionId.GetOrCreateItem(stagingRecord.SessionId, 
                    () => new SessionRecords { MinEventId = stagingRecord.EventId, Records = new List<MediationRecord>() });
                sessionRecords.Records.Add(stagingRecord);
                if (stagingRecord.EventId < sessionRecords.MinEventId)
                    sessionRecords.MinEventId = stagingRecord.EventId;
            }
            foreach(var sessionRecords in recordsBySessionId.Values.OrderBy(itm => itm.MinEventId))
            {
                inputArgument.MediationRecords.Enqueue(new MediationRecordBatch() { MediationRecords = sessionRecords.Records });
            }
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.MediationRecords.Get(context) == null)
                this.MediationRecords.Set(context, new MemoryQueue<MediationRecordBatch>());
            if (this.EventIds.Get(context) == null)
                this.EventIds.Set(context, new HashSet<int>());
            if (this.MediationDefinition.Get(context) == null)
                this.MediationDefinition.Set(context, new MediationDefinition());
            base.OnBeforeExecute(context, handle);
        }

        private class SessionRecords
        {
            public long MinEventId { get; set; }

            public List<MediationRecord> Records { get; set; }
        }
    }
}
