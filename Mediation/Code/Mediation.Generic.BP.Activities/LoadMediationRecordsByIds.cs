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
using Vanrise.Entities;

namespace Mediation.Generic.BP.Activities
{
    public class LoadMediationRecordsByIdsInput
    {
        public BaseQueue<SessionIdsBatch> SessionIds { get; set; }
        public BaseQueue<MediationRecordBatch> MediationRecords { get; set; }
        public MediationDefinition MediationDefinition { get; set; }
    }

    public sealed class LoadMediationRecordsByIds : DependentAsyncActivity<LoadMediationRecordsByIdsInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<SessionIdsBatch>> SessionIds { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<MediationRecordBatch>> MediationRecords { get; set; }

        [RequiredArgument]
        public InArgument<MediationDefinition> MediationDefinition { get; set; }

        protected override LoadMediationRecordsByIdsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new LoadMediationRecordsByIdsInput
            {
                SessionIds = this.SessionIds.Get(context),
                MediationRecords = this.MediationRecords.Get(context),
                MediationDefinition = this.MediationDefinition.Get(context)
            };
        }

        protected override void DoWork(LoadMediationRecordsByIdsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Start Loading Mediation Records By Id Activity");

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
             {
                 bool hasItems = false;
                 do
                 {
                     hasItems = inputArgument.SessionIds.TryDequeue((sessionIdsBatch) =>
                      {
                          MediationRecordsManager manager = new MediationRecordsManager();
                          var mediationRecords = manager.GetMediationRecordsByIds(inputArgument.MediationDefinition.MediationDefinitionId, sessionIdsBatch.SessionIds, inputArgument.MediationDefinition.ParsedRecordTypeId, sessionIdsBatch.LastCommittedId.Value);
                          handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "{0} Mediation Records are loaded", mediationRecords.Count());

                          Dictionary<string, SessionRecords> recordsBySessionId = new Dictionary<string, SessionRecords>();
                          foreach (var stagingRecord in mediationRecords)
                          {
                              SessionRecords sessionRecords = recordsBySessionId.GetOrCreateItem(stagingRecord.SessionId,
                                  () => new SessionRecords { MinEventId = stagingRecord.EventId, Records = new List<MediationRecord>() });
                              sessionRecords.Records.Add(stagingRecord);
                             
                              if (stagingRecord.EventId < sessionRecords.MinEventId)
                                  sessionRecords.MinEventId = stagingRecord.EventId;//MinEventId is required to ensure sequential processing before feeding any second stage mediation
                          }
                          foreach (var sessionRecordsEntry in recordsBySessionId.OrderBy(itm => itm.Value.MinEventId))
                          {
                              inputArgument.MediationRecords.Enqueue(new MediationRecordBatch() { LastCommittedId = sessionIdsBatch.LastCommittedId.Value, SessionId = sessionRecordsEntry.Key, MediationRecords = sessionRecordsEntry.Value.Records });
                          }
                      });

                 } while (!ShouldStop(handle) && hasItems);
             });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "End Loading Mediation Records By Id Activity");

        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.MediationRecords.Get(context) == null)
                this.MediationRecords.Set(context, new MemoryQueue<MediationRecordBatch>());
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
