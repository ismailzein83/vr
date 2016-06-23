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
    public class LoadStoreStagingRecordsByIdsInput
    {
        public IEnumerable<int> EventIds { get; set; }
        public BaseQueue<StoreStagingRecordBatch> StoreStagingRecords { get; set; }
        public MediationDefinition MediationDefinition { get; set; }
    }


    public sealed class LoadStoreStagingRecordsByIds : DependentAsyncActivity<LoadStoreStagingRecordsByIdsInput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<int>> EventIds { get; set; }

        [RequiredArgument]
        public OutArgument<BaseQueue<StoreStagingRecordBatch>> StoreStagingRecords { get; set; }

        [RequiredArgument]
        public InArgument<MediationDefinition> MediationDefinition { get; set; }

        protected override LoadStoreStagingRecordsByIdsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new LoadStoreStagingRecordsByIdsInput
            {
                EventIds = this.EventIds.Get(context),
                StoreStagingRecords = this.StoreStagingRecords.Get(context),
                MediationDefinition = this.MediationDefinition.Get(context)
            };
        }

        protected override void DoWork(LoadStoreStagingRecordsByIdsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            StoreStagingRecordsManager manager = new StoreStagingRecordsManager();

            var storeStagingRecords = manager.GetStoreStagingRecordsByIds(inputArgument.EventIds);
            long sessionId = 0;
            List<StoreStagingRecord> records = new List<StoreStagingRecord>();
            foreach (var stagingRecord in storeStagingRecords)
            {
                records.Add(stagingRecord);
                if (sessionId != stagingRecord.SessionId)
                {
                    sessionId = stagingRecord.SessionId;
                    inputArgument.StoreStagingRecords.Enqueue(new StoreStagingRecordBatch() { StoreStagingRecords = records });
                    records = new List<StoreStagingRecord>();
                }
            }
            if (records.Count > 0)
            {
                inputArgument.StoreStagingRecords.Enqueue(new StoreStagingRecordBatch() { StoreStagingRecords = records });
            }
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.StoreStagingRecords.Get(context) == null)
                this.StoreStagingRecords.Set(context, new MemoryQueue<StoreStagingRecordBatch>());
            if (this.EventIds.Get(context) == null)
                this.EventIds.Set(context, new HashSet<int>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
