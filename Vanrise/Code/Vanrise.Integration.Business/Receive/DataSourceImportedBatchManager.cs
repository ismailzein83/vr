using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceImportedBatchManager
    {
        public long WriteEntry(Entities.ImportedBatchEntry entry, Guid dataSourceId, string logEntryTime)
        {
            IDataSourceImportedBatchDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return manager.InsertEntry(dataSourceId, entry.BatchDescription, entry.BatchSize, entry.RecordsCount, entry.Result, entry.MapperMessage, entry.QueueItemsIds, logEntryTime);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
        {
            IDataSourceImportedBatchDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            Vanrise.Entities.BigResult<DataSourceImportedBatch> bigResult = dataManager.GetFilteredDataSourceImportedBatches(input);

            //Technically no need to use Hashset because ids in seperate rows will not repeat. 
            //Just in case it did, the use of hashset is to avoid this problem.
            HashSet<long> queueItemIds = new HashSet<long>();
            foreach (DataSourceImportedBatch batch in bigResult.Data)
            {
                if(batch.QueueItemIds.Contains(','))
                {
                    string [] qIds = batch.QueueItemIds.Split(',');
                    foreach (string qId in qIds)
                    {
                        queueItemIds.Add(long.Parse(qId));
                    }
                }
                else
                {
                    if(!string.IsNullOrEmpty(batch.QueueItemIds))
                        queueItemIds.Add(long.Parse(batch.QueueItemIds)); 
                }
            }

            QueueingManager qManager = new QueueingManager();
            Dictionary<long, ItemExecutionFlowInfo> dicItemExecutionStatus = qManager.GetItemsExecutionFlowStatus(queueItemIds.ToList());

            foreach (DataSourceImportedBatch batch in bigResult.Data)
            {
                if (batch.QueueItemIds.Contains(','))
                {
                    string[] qIds = batch.QueueItemIds.Split(',');
                    List<ItemExecutionFlowInfo> list = new List<ItemExecutionFlowInfo>();
                    
                    foreach (string qId in qIds)
                    {
                        long singleQueueItemId = long.Parse(qId);
                        ItemExecutionFlowInfo itemExecutionFlowInfo;
                        if (dicItemExecutionStatus.TryGetValue(singleQueueItemId, out itemExecutionFlowInfo))
                            list.Add(itemExecutionFlowInfo);
                    }
                    
                    batch.ExecutionStatus = qManager.GetExecutionFlowStatus(list);
                }
                else
                {
                    if (string.IsNullOrEmpty(batch.QueueItemIds))
                        batch.ExecutionStatus = ItemExecutionFlowStatus.Failed;
                    else
                        batch.ExecutionStatus = dicItemExecutionStatus[long.Parse(batch.QueueItemIds)].Status;
                }
            }
            
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, bigResult);
        }
    }
}
