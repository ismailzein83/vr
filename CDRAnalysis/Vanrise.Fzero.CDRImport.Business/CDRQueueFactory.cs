using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.Business
{
    public static class CDRQueueFactory
    {
        public static PersistentQueue<ImportedCDRBatch> GetImportedCDRQueue(string dataSourceKey)
        {
            return GetSwitchQueue<ImportedCDRBatch>(dataSourceKey, QueueName.ImportedCDRs);
        }

        public static PersistentQueue<ImportedCDRBatch> GetParsedCDRQueue(string dataSourceKey)
        {
            return GetSwitchQueue<ImportedCDRBatch>(dataSourceKey, QueueName.ImportedCDRs);
        }

        private static PersistentQueue<T> GetSwitchQueue<T>(string dataSourceKey, QueueName queueNameTemplate) where T : PersistentQueueItem
        {
            CreateAllSwitchQueuesIfNotExists(dataSourceKey);
            string queueName = String.Format("{0}_{1}", queueNameTemplate, dataSourceKey);
            return PersistentQueueFactory.Default.GetQueue<T>(queueName);
        }

        static void CreateAllSwitchQueuesIfNotExists(string dataSourceKey)
        {
            if (s_CreatedSwitchesQueues.Contains(dataSourceKey))
                return;
            lock (s_CreatedSwitchesQueues)
            {
                if (s_CreatedSwitchesQueues.Contains(dataSourceKey))
                    return;

                CreateSwitchQueueIfNotExists<ImportedCDRBatch>(dataSourceKey, QueueName.ImportedCDRs);
                CreateSwitchQueueIfNotExists<ParsedCDRBatch>(dataSourceKey, QueueName.ParsedCDRs);
            }
        }

        private static void CreateSwitchQueueIfNotExists<T>(string dataSourceKey, QueueName queueNameTemplate, QueueName? sourceQueueNameTemplate = null, QueueSettings queueSettings = null) where T : PersistentQueueItem
        {
            string queueName = String.Format("{0}_{1}", queueNameTemplate, dataSourceKey);
            QueueNameAttribute queueNameAtt = Vanrise.Common.Utilities.GetEnumAttribute<QueueName, QueueNameAttribute>(queueNameTemplate);
            string queueTitle = String.Format(queueNameAtt.QueueTitle, dataSourceKey);
            string[] sourceQueueNames = null;
            if (sourceQueueNameTemplate != null)
                sourceQueueNames = new string[] { String.Format("{0}_{1}", sourceQueueNameTemplate, dataSourceKey) };
            PersistentQueueFactory.Default.CreateQueueIfNotExists<T>(0, queueName, queueTitle, sourceQueueNames, queueSettings);
        }

        static List<string> s_CreatedSwitchesQueues = new List<string>();
    }
}
