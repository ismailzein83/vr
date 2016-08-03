using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class SummaryBatchActivatorDataManager : BaseSQLDataManager, ISummaryBatchActivatorDataManager
    {
        public SummaryBatchActivatorDataManager()
            : base(GetConnectionStringName("QueueDBConnStringKey", "QueueDBConnString"))
        {
        }

        public List<SummaryBatchActivator> GetAllSummaryBatchActivators()
        {
            return GetItemsSP("[queue].[sp_SummaryBatchActivator_GetAll]", SummaryBatchActivatorMapper);
        }

        public List<SummaryBatchActivator> GetSummaryBatchActivators(Guid activatorId)
        {
            return GetItemsSP("[queue].[sp_SummaryBatchActivator_GetByActivator]", SummaryBatchActivatorMapper, activatorId);
        }

        public void Delete(int queueId, DateTime batchStart)
        {
            ExecuteNonQuerySP("[queue].[sp_SummaryBatchActivator_Delete]", queueId, batchStart);
        }

        public void Insert(List<SummaryBatchActivator> summaryBatchActivators)
        {
            foreach(var summaryBatchActivator in summaryBatchActivators)
            {
                ExecuteNonQuerySP("[queue].[sp_SummaryBatchActivator_Insert]", summaryBatchActivator.QueueId, summaryBatchActivator.BatchStart, summaryBatchActivator.ActivatorId);
            }
        }

        #region Mappers

        private SummaryBatchActivator SummaryBatchActivatorMapper(IDataReader reader)
        {
            return new SummaryBatchActivator
            {
                QueueId = (int)reader["QueueID"],
                BatchStart = (DateTime)reader["BatchStart"],
                ActivatorId = (Guid)reader["ActivatorID"]
            };
        }

        #endregion
    }
}
