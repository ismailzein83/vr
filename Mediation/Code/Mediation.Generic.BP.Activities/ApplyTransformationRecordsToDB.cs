using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;

namespace Mediation.Generic.BP.Activities
{
    public class ApplyTransformationRecordsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public Guid DataRecordStorageId { get; set; }
    }
    public sealed class ApplyTransformationRecordsToDB : DependentAsyncActivity<ApplyTransformationRecordsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<Guid> DataRecordStorageId { get; set; }

        protected override void DoWork(ApplyTransformationRecordsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
            IDataRecordDataManager recordStorageDataManager = dataRecordStorageManager.GetStorageDataManager(inputArgument.DataRecordStorageId);
            if (recordStorageDataManager == null)
                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", inputArgument.DataRecordStorageId));
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue((preparedCdrBatch) =>
                    {
                        recordStorageDataManager.ApplyStreamToDB(preparedCdrBatch);
                    });
                } while (!ShouldStop(handle) && hasItems);
            });
        }

        protected override ApplyTransformationRecordsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyTransformationRecordsToDBInput()
            {
                DataRecordStorageId = this.DataRecordStorageId.Get(context),
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
