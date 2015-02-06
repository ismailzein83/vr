using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public static class PersistentQueueFactory
    {
        public static PersistentQueue<T> GetQueue<T>(string queueName) where T : PersistentQueueItem
        {
            IQueueDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            QueueInstance queueInstance = dataManager.GetQueueInstance(queueName);
            if (queueInstance == null)
                return null;

            Type queueItemType = Type.GetType(queueInstance.ItemFQTN);
            if (typeof(T) != queueItemType)
                throw new Exception(String.Format("Queue '{0}' is not of type {1}. Expected Item Type {2}", queueName, typeof(T), queueItemType));

            return new PersistentQueue<T>(queueInstance.QueueInstanceId, queueInstance.Settings);
        }

        public static void CreateQueue<T>(string queueName, string queueTitle, IEnumerable<string> sourceQueueNames, bool singleConcurrentReader = false) where T : PersistentQueueItem
        {
            IQueueDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            QueueSettings queueSettings = new QueueSettings
            {
                SingleConcurrentReader = singleConcurrentReader
            };
            string itemFQTN = typeof(T).AssemblyQualifiedName;
            List<int> sourceQueueIds = new List<int>();
            if (sourceQueueNames != null)
            {
                foreach (var sourceQueueName in sourceQueueNames)
                {
                    QueueInstance sourceQueue = dataManager.GetQueueInstance(sourceQueueName);
                    if (sourceQueue == null)
                        throw new Exception(String.Format("Source Queue '{0}' not found", sourceQueueName));
                    if (!sourceQueueIds.Contains(sourceQueue.QueueInstanceId))
                        sourceQueueIds.Add(sourceQueue.QueueInstanceId);
                }
            }
            dataManager.CreateQueue(queueName, queueTitle, itemFQTN, queueSettings, sourceQueueIds);
        }

        public static void CreateQueue<T>(string queueName, string queueTitle, bool singleConcurrentReader = false) where T : PersistentQueueItem
        {
            CreateQueue<T>(queueName, queueTitle, null, singleConcurrentReader);
        }

        public static void CreateQueue<T>(string queueName, bool singleConcurrentReader = false) where T : PersistentQueueItem
        {
            CreateQueue<T>(queueName, queueName, null, singleConcurrentReader);
        }
    }
}