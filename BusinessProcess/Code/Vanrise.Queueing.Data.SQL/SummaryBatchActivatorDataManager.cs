using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
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
            DateTimeRange dateTimeRange = base.GetSQLDateTimeRange();

            foreach (var summaryBatchActivator in summaryBatchActivators)
            {
                DateTime batchStart = summaryBatchActivator.BatchStart;
                if (CheckIfDefaultOrInvalid(summaryBatchActivator.BatchStart))
                    batchStart = dateTimeRange.From;

                DateTime batchEnd = summaryBatchActivator.BatchEnd;
                if (CheckIfDefaultOrInvalid(summaryBatchActivator.BatchEnd))
                    batchEnd = dateTimeRange.From;

                ExecuteNonQuerySP("[queue].[sp_SummaryBatchActivator_Insert]", summaryBatchActivator.QueueId, batchStart, batchEnd, summaryBatchActivator.ActivatorId);
            }
        }

        public bool HasPendingSummaryBatchActivators(List<int> queueIdsToCheck, DateTime from, DateTime to)
        {
            string queueIdsToCheckAsString = null;
            if (queueIdsToCheck != null)
                queueIdsToCheckAsString = string.Join<int>(",", queueIdsToCheck);

            object hasPendingItems;
            ExecuteNonQuerySP("[queue].[sp_SummaryBatchActivator_HasPendingItems]", out hasPendingItems, queueIdsToCheckAsString, from, to);
            return (bool)hasPendingItems;
        }

        #region Mappers

        private SummaryBatchActivator SummaryBatchActivatorMapper(IDataReader reader)
        {
            return new SummaryBatchActivator
            {
                QueueId = (int)reader["QueueID"],
                BatchStart = (DateTime)reader["BatchStart"],
                BatchEnd = (DateTime)reader["BatchEnd"],
                ActivatorId = (Guid)reader["ActivatorID"]
            };
        }

        #endregion
    }
}