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
    public class QueueSubscriptionDataManager : BaseSQLDataManager, IQueueSubscriptionDataManager
    {
        public QueueSubscriptionDataManager()
            : base(GetConnectionStringName("QueueingConfigDBConnStringKey", "QueueingConfigDBConnString"))
        {
        }

        public void InsertSubscription(IEnumerable<int> sourceQueueIds, int susbscribedQueueId)
        {
            if (sourceQueueIds != null)
            {
                foreach (var sourceQueueId in sourceQueueIds)
                {
                    ExecuteNonQuerySP("queue.sp_QueueSubscription_Insert", sourceQueueId, susbscribedQueueId);
                }
            }
        }

        public List<QueueSubscription> GetSubscriptions()
        {
            return GetItemsSP("queue.sp_QueueSubscription_GetAll", QueueSubscriptionMapper, (int)QueueInstanceStatus.ReadyToUse);
        }

        #region Private Methods

        private QueueSubscription QueueSubscriptionMapper(IDataReader reader)
        {
            return new QueueSubscription
            {
                QueueID = (int)reader["QueueID"],
                SubsribedQueueID = (int)reader["SubscribedQueueID"]
            };
        }

        #endregion


        public bool AreSubscriptionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("queue.QueueSubscription", ref updateHandle);
        }
    }
}
