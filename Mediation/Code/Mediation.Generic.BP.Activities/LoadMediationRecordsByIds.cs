using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Mediation.Generic.Business;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

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
            string sessionId = "";
            Dictionary<string, List<MediationRecord>> records = new Dictionary<string, List<MediationRecord>>();
            List<MediationRecord> lstMediationRecords;
            foreach (var stagingRecord in mediationRecords)
            {
                if (!records.TryGetValue(stagingRecord.SessionId, out lstMediationRecords))
                {
                    if (!string.IsNullOrEmpty(sessionId))
                        inputArgument.MediationRecords.Enqueue(new MediationRecordBatch() { MediationRecords = records[sessionId] });
                    sessionId = stagingRecord.SessionId;
                    lstMediationRecords = new List<MediationRecord>();
                    records.Add(stagingRecord.SessionId, lstMediationRecords);
                }
                lstMediationRecords.Add(stagingRecord);
            }
            if (records.TryGetValue(sessionId, out lstMediationRecords))
                inputArgument.MediationRecords.Enqueue(new MediationRecordBatch() { MediationRecords = lstMediationRecords });
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
    }
}
