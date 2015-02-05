using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueItemHeaderDataManager: BaseSQLDataManager, IQueueItemHeaderDataManager
    {
        public QueueItemHeaderDataManager()
            : base(ConfigurationManager.AppSettings["QueueingDBConnStringKey"] ?? "QueueingDBConnString")
        {
        }

        public QueueItemHeader Get(Guid itemId)
        {
            return GetItemSP("queue.sp_QueueItemHeader_GetByID", QueueItemHeaderMapper, itemId);
        }        

        public void Insert(Guid itemId, int queueId, string description, QueueItemStatus queueItemStatus)
        {
            ExecuteNonQuerySP("queue.sp_QueueItemHeader_Insert", itemId, queueId, description, (int)queueItemStatus);
        }

        public void UpdateStatus(Guid itemId, QueueItemStatus queueItemStatus)
        {
            ExecuteNonQuerySP("queue.sp_QueueItemHeader_UpdateStatus", itemId, (int)queueItemStatus);
        }

        public void Update(Guid itemId, QueueItemStatus queueItemStatus, int retryCount, string errorMessage)
        {
            ExecuteNonQuerySP("queue.sp_QueueItemHeader_Update", itemId, (int)queueItemStatus, retryCount, errorMessage);
        }

        #region Private Methods

        private QueueItemHeader QueueItemHeaderMapper(IDataReader reader)
        {
            return new QueueItemHeader
            {
                Id = (Guid)reader["ID"],
                QueueId = (int)reader["QueueID"],
                Description = reader["Description"] as string,
                Status = (QueueItemStatus)reader["Status"],
                RetryCount = GetReaderValue<int>(reader, "RetryCount"),
                ErrorMessage = reader["ErrorMessage"] as string,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                LastUpdatedTime = GetReaderValue<DateTime>(reader, "LastUpdatedTime")
            };
        }

        #endregion
    }
}
