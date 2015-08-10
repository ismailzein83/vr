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
        public long WriteEntry(Entities.ImportedBatchEntry entry, int dataSourceId, DateTime logEntryTime)
        {
            IDataSourceImportedBatchDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return manager.InsertEntry(dataSourceId, entry.BatchDescription, entry.BatchSize, entry.RecordsCount, entry.Result, entry.MapperMessage, entry.QueueItemsIds, logEntryTime);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
        {
            IDataSourceImportedBatchDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            Vanrise.Entities.BigResult<DataSourceImportedBatch> bigResult = dataManager.GetFilteredDataSourceImportedBatches(input);

            List<long> queueItemIds = new List<long>();
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
                    queueItemIds.Add(long.Parse(batch.QueueItemIds)); 
                }
            }

            QueueingManager qManager = new QueueingManager();
            Dictionary<long, ItemExecutionFlowInfo> dicItemExecutionStatus = qManager.GetItemsExecutionFlowStatus(queueItemIds);

            foreach (DataSourceImportedBatch batch in bigResult.Data)
            {
                if (batch.QueueItemIds.Contains(','))
                {
                    string[] qIds = batch.QueueItemIds.Split(',');
                    foreach (string qId in qIds)
                    {
                        long singleQueueItemId = long.Parse(qId);
                        List<ItemExecutionFlowInfo> list = new List<ItemExecutionFlowInfo>();
                        list.Add(dicItemExecutionStatus[singleQueueItemId]);
                        batch.ExecutionStatus = qManager.GetExecutionFlowStatus(list);
                    }
                }
                else
                {
                    batch.ExecutionStatus = dicItemExecutionStatus[long.Parse(batch.QueueItemIds)].Status;
                }
            }
            
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, bigResult);
        }

        public Vanrise.Entities.IDataRetrievalResult<QueueItemHeader> GetQueueItemHeaders(Vanrise.Entities.DataRetrievalInput<QueueItemHeaderQuery> input)
        {
            Vanrise.Entities.BigResult<QueueItemHeader> bigResult = new Vanrise.Entities.BigResult<QueueItemHeader>();

            QueueingManager manager = new QueueingManager();
            bigResult.Data = manager.GetQueueItemsHeader(input.Query.ItemIds);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, bigResult);
        }
    }
}
