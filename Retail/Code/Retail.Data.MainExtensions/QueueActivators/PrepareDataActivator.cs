using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Queueing.Entities;
using Vanrise.Reprocess.Entities;
using Vanrise.Common;

namespace Retail.Data.MainExtensions.QueueActivators
{
    public class PrepareDataActivator : QueueActivator, IReprocessStageActivator
    {
        #region QueueActivator

        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
        }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            //DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            //var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            //if (queueItemType == null)
            //    throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            //var recordTypeId = queueItemType.DataRecordTypeId;
            //var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            //DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            //Type accumulatedDataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(_accumulatedDataRecordTypeId);

            //DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
            //int maxParameterNumber = 51000; // dataRecordStorageManager.GetDBQueryMaxParameterNumber(_accumulatedDataRecordStorageId);

            //Dictionary<string, dynamic> accumulatedDataDict = new Dictionary<string, dynamic>();
            //Dictionary<string, dynamic> accumulatedDataToAddDict = new Dictionary<string, dynamic>();
            //Dictionary<string, dynamic> accumulatedDataToUpdateDict = new Dictionary<string, dynamic>();

            //Dictionary<string, List<dynamic>> batchRecordsByUserSession = new Dictionary<string, List<dynamic>>();
            //foreach (var record in batchRecords)
            //{
            //    List<dynamic> dataRecords = batchRecordsByUserSession.GetOrCreateItem(record.UserSession as string, () => { return new List<dynamic>(); });
            //    dataRecords.Add(record);
            //}

            //List<dynamic> userSessionsData = new List<dynamic>();
            //foreach (var kvp in batchRecordsByUserSession)
            //{
            //    userSessionsData.Add(kvp.Key);

            //    if (userSessionsData.Count >= maxParameterNumber)
            //    {
            //        AddUserSessionsData(userSessionsData);
            //        userSessionsData = new List<dynamic>();
            //    }
            //}

            //if (userSessionsData.Count > 0)
            //    AddUserSessionsData(userSessionsData);

        }

        private void AddUserSessionsData(List<dynamic> userSessionData)
        {
        }

        #endregion

        #region IReprocessStageActivator

        public void CommitChanges(IReprocessStageActivatorCommitChangesContext context)
        {
            throw new NotImplementedException();
        }

        public void DropStorage(IReprocessStageActivatorDropStorageContext context)
        {
            throw new NotImplementedException();
        }

        public void ExecuteStage(IReprocessStageActivatorExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void FinalizeStage(IReprocessStageActivatorFinalizingContext context)
        {
            throw new NotImplementedException();
        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Queueing.BaseQueue<IReprocessBatch> GetQueue()
        {
            throw new NotImplementedException();
        }

        public List<BatchRecord> GetStageBatchRecords(IReprocessStageActivatorPreparingContext context)
        {
            throw new NotImplementedException();
        }

        public int? GetStorageRowCount(IReprocessStageActivatorGetStorageRowCountContext context)
        {
            throw new NotImplementedException();
        }

        public object InitializeStage(IReprocessStageActivatorInitializingContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
