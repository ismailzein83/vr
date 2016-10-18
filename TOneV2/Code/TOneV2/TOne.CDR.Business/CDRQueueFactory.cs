using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Entities;
using TOne.CDRProcess.Arguments;
using TOne.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.Business
{
    public static class CDRQueueFactory
    {
        public static PersistentQueue<CDRBatch> GetCDRQueue(int switchId)
        {
            return GetSwitchQueue<CDRBatch>(switchId, QueueName.CDRRaw);
        }

        public static PersistentQueue<CDRBatch> GetCDRForBillingQueue(int switchId)
        {
            return GetSwitchQueue<CDRBatch>(switchId, QueueName.CDRRawForBilling);
        }

        public static PersistentQueue<CDRBillingBatch> GetCDRBillingQueue(int switchId)
        {
            return GetSwitchQueue<CDRBillingBatch>(switchId, QueueName.CDRBilling);
        }

        public static PersistentQueue<CDRBillingBatch> GetCDRBillingForStatisticsQueue(int switchId)
        {
            return GetSwitchQueue<CDRBillingBatch>(switchId, QueueName.CDRBillingForStats);
        }

        public static PersistentQueue<CDRBillingBatch> GetCDRBillingForStatisticsDailyQueue(int switchId)
        {
            return GetSwitchQueue<CDRBillingBatch>(switchId, QueueName.CDRBillingForStatsDaily);
        }

        public static PersistentQueue<CDRMainBatch> GetCDRMainQueue(int switchId)
        {
            return GetSwitchQueue<CDRMainBatch>(switchId, QueueName.CDRMain);
        }

        public static PersistentQueue<CDRInvalidBatch> GetCDRInvalidQueue(int switchId)
        {
            return GetSwitchQueue<CDRInvalidBatch>(switchId, QueueName.CDRInvalid);
        }

        public static PersistentQueue<TrafficStatisticBatch> GetTrafficStatsQueue(int switchId)
        {
            return GetSwitchQueue<TrafficStatisticBatch>(switchId, QueueName.TrafficStats);
        }

        public static PersistentQueue<TrafficStatisticDailyBatch> GetTrafficStatsDailyQueue(int switchId)
        {
            return GetSwitchQueue<TrafficStatisticDailyBatch>(switchId, QueueName.TrafficStatsDaily);
        }

        private static PersistentQueue<T> GetSwitchQueue<T>(int switchId, QueueName queueNameTemplate) where T : PersistentQueueItem
        {
            CreateAllSwitchQueuesIfNotExists(switchId);
            string queueName = String.Format("{0}_{1}", queueNameTemplate, switchId);
            return PersistentQueueFactory.Default.GetQueue<T>(queueName);
        }

        static void CreateAllSwitchQueuesIfNotExists(int switchId)
        {
            if (s_CreatedSwitchesQueues.Contains(switchId))
                return;
            lock (s_CreatedSwitchesQueues)
            {
                if (s_CreatedSwitchesQueues.Contains(switchId))
                    return;

                CreateSwitchQueueIfNotExists<CDRBatch>(switchId, QueueName.CDRRaw, null, new QueueSettings {});
                CreateSwitchQueueIfNotExists<CDRBatch>(switchId, QueueName.CDRRawForBilling, QueueName.CDRRaw, new QueueSettings {  });

                CreateSwitchQueueIfNotExists<CDRBillingBatch>(switchId, QueueName.CDRBilling, null, new QueueSettings {   });
                CreateSwitchQueueIfNotExists<CDRBillingBatch>(switchId, QueueName.CDRBillingForStats, QueueName.CDRBilling , new QueueSettings { });
                CreateSwitchQueueIfNotExists<CDRBillingBatch>(switchId, QueueName.CDRBillingForStatsDaily, QueueName.CDRBilling, new QueueSettings {  });

                CreateSwitchQueueIfNotExists<CDRMainBatch>(switchId, QueueName.CDRMain, null, new QueueSettings { });
                CreateSwitchQueueIfNotExists<CDRInvalidBatch>(switchId, QueueName.CDRInvalid, null, new QueueSettings { });
                CreateSwitchQueueIfNotExists<TrafficStatisticBatch>(switchId, QueueName.TrafficStats, null, new QueueSettings { });
                CreateSwitchQueueIfNotExists<TrafficStatisticDailyBatch>(switchId, QueueName.TrafficStatsDaily, null, new QueueSettings { });
                
                s_CreatedSwitchesQueues.Add(switchId);
            }
        }

        private static void CreateSwitchQueueIfNotExists<T>(int switchId, QueueName queueNameTemplate, QueueName? sourceQueueNameTemplate = null, QueueSettings queueSettings = null) where T : PersistentQueueItem
        {
            string queueName = String.Format("{0}_{1}", queueNameTemplate, switchId);
            QueueNameAttribute queueNameAtt = Vanrise.Common.Utilities.GetEnumAttribute<QueueName, QueueNameAttribute>(queueNameTemplate);
            string queueTitle = String.Format(queueNameAtt.QueueTitle, TABS.Switch.All[(byte)switchId].Name);
            string[] sourceQueueNames = null;
            if (sourceQueueNameTemplate != null)
                sourceQueueNames = new string[] { String.Format("{0}_{1}", sourceQueueNameTemplate, switchId) };
            PersistentQueueFactory.Default.CreateQueueIfNotExists(Guid.Empty, null, typeof(T).AssemblyQualifiedName, queueName, queueTitle, sourceQueueNames, queueSettings);
        }

        static List<int> s_CreatedSwitchesQueues = new List<int>();
    }
}