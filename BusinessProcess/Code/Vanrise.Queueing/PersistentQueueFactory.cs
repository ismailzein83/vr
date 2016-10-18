using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class PersistentQueueFactory
    {
        #region Singleton

        static PersistentQueueFactory()
        {
            _default = new PersistentQueueFactory();
        }


        static PersistentQueueFactory _default;
        public static PersistentQueueFactory Default
        {
            get
            {
                return _default;
            }
        }

        private PersistentQueueFactory()
        {

        }

        QueueInstanceManager _queueManager = new QueueInstanceManager();

        #endregion
        public PersistentQueue<T> GetQueue<T>(string queueName) where T : PersistentQueueItem
        {
            string cacheName = String.Format("PersistentQueueFactory_GetQueue<T>_{0}_{1}", typeof(T).FullName, queueName);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceManager.CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    QueueInstance queueInstance = _queueManager.GetQueueInstance(queueName);
                    if (queueInstance == null || queueInstance.Status != QueueInstanceStatus.ReadyToUse)
                        return null;

                    Type queueItemType = Type.GetType(queueInstance.ItemFQTN);
                    if (typeof(T) != queueItemType)
                        throw new Exception(String.Format("Queue '{0}' is of item type {1}. Expected Item Type {2}", queueName, queueItemType, typeof(T)));

                    return new PersistentQueue<T>(queueInstance.QueueInstanceId, queueInstance.Settings);
                });
        }

        public IPersistentQueue GetQueue(string queueName)
        {
            string cacheName = String.Format("PersistentQueueFactory_GetQueueByName_{0}", queueName);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceManager.CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    QueueInstance queueInstance = _queueManager.GetQueueInstance(queueName);
                    return BuildPersistentQueue(queueInstance);
                });
           
        }

        public IPersistentQueue GetQueue(int queueInstanceId)
        {
            string cacheName = String.Format("PersistentQueueFactory_GetQueueById_{0}", queueInstanceId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceManager.CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    QueueInstance queueInstance = _queueManager.GetQueueInstanceById(queueInstanceId);
                    return BuildPersistentQueue(queueInstance);
                });

        }

        public bool QueueExists(string queueName)
        {
            QueueInstance queueInstance = _queueManager.GetQueueInstance(queueName);
            return queueInstance != null && queueInstance.Status == QueueInstanceStatus.ReadyToUse;
        }

        public void CreateQueue(Guid executionFlowId, string stageName, string queueItemFQTN, string queueName, string queueTitle, IEnumerable<string> sourceQueueNames, QueueSettings queueSettings = null)
        {
            IQueueDataManager dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            IQueueItemDataManager dataManagerQueueItem = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            QueueSubscriptionManager queueSubscriptionManager = new QueueSubscriptionManager();
            Type queueItemType = Type.GetType(queueItemFQTN);
            if (queueItemType == null)
                throw new Exception(String.Format("type '{0}' is not available", queueItemFQTN));

            PersistentQueueItem emptyInstance = Activator.CreateInstance(queueItemType) as PersistentQueueItem;
            if (emptyInstance == null)
                throw new Exception(String.Format("{0} is not of type PersistentQueueItem", queueItemFQTN));
            List<int> sourceQueueIds = new List<int>();
            if (sourceQueueNames != null)
            {
                foreach (var sourceQueueName in sourceQueueNames)
                {
                    QueueInstance sourceQueue = _queueManager.GetQueueInstance(sourceQueueName);
                    if (sourceQueue == null)
                        throw new Exception(String.Format("Source Queue '{0}' not found", sourceQueueName));

                    if (sourceQueue.Status != QueueInstanceStatus.ReadyToUse)
                        throw new Exception(String.Format("Source Queue '{0}' is not ready to use", sourceQueueName));

                    Type sourceQueueItemType = Type.GetType(sourceQueue.ItemFQTN);
                    if (queueItemType != sourceQueueItemType)
                        throw new Exception(String.Format("Source Queue '{0}' is of item type {1}. Expected Item Type {2}", sourceQueueName, sourceQueueItemType, queueItemFQTN));

                    if (!sourceQueueIds.Contains(sourceQueue.QueueInstanceId))
                        sourceQueueIds.Add(sourceQueue.QueueInstanceId);
                }
            }
            int itemTypeId = dataManagerQueue.InsertOrUpdateQueueItemType(queueItemFQTN, emptyInstance.ItemTypeTitle, emptyInstance.DefaultQueueSettings);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueItemTypeManager.CacheManager>().SetCacheExpired();
            Action createQueueAction = () =>
            {
                int queueId = dataManagerQueue.InsertQueueInstance(executionFlowId, stageName, queueName, queueTitle, QueueInstanceStatus.New, itemTypeId, queueSettings);
                queueSubscriptionManager.AddSubscriptions(sourceQueueIds, queueId);
                dataManagerQueueItem.CreateQueue(queueId);
                dataManagerQueue.UpdateQueueInstanceStatus(queueName, QueueInstanceStatus.ReadyToUse);
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceManager.CacheManager>().SetCacheExpired();               
            };
            //this logic is implemented to rename any previous failed Create Queue action to allow recreation. this is done instead of using Transaction against two databases
            try
            {
                createQueueAction();
            }
            catch
            {
                string inactiveQueueName = String.Format("{0}_Inactive_{1}", queueName, Guid.NewGuid());
                if (dataManagerQueue.UpdateQueueName(queueName, QueueInstanceStatus.New, inactiveQueueName)
                    || dataManagerQueue.UpdateQueueName(queueName, QueueInstanceStatus.Deleted, inactiveQueueName))
                    createQueueAction();
                else
                    throw;
            }
        }

        public void CreateQueueIfNotExists(Guid executionFlowId, string stageName, string queueItemFQTN, string queueName, string queueTitle = null, IEnumerable<string> sourceQueueNames = null, QueueSettings queueSettings = null)
        {
            queueTitle = queueTitle ?? queueName;            
            if (!QueueExists(queueName))
            {
                try
                {
                    CreateQueue(executionFlowId, stageName, queueItemFQTN, queueName, queueTitle, sourceQueueNames, queueSettings);
                }
                catch
                {
                    if (!QueueExists(queueName))
                        throw;
                }
            }
            else
            {
                QueueInstance queueInstance = _queueManager.GetQueueInstance(queueName);
                if (queueInstance.StageName != stageName || queueInstance.Title != queueTitle || !AreSameSettings(queueInstance.Settings, queueSettings))
                {
                    IQueueDataManager dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
                    dataManagerQueue.UpdateQueueInstance(queueName, stageName, queueTitle, queueSettings);
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceManager.CacheManager>().SetCacheExpired();
                }
            }
        }

        private bool AreSameSettings(QueueSettings queueSettings1, QueueSettings queueSettings2)
        {
            if(queueSettings1 == null || queueSettings2 == null)
            {
                return queueSettings1 == queueSettings2;
            }
            else
            {
                return Serializer.Serialize(queueSettings1) == Serializer.Serialize(queueSettings2);
            }
        }

        private IPersistentQueue BuildPersistentQueue(QueueInstance queueInstance)
        {
            if (queueInstance == null || queueInstance.Status != QueueInstanceStatus.ReadyToUse)
                return null;
            Type queueItemType = Type.GetType(queueInstance.ItemFQTN);
            Type[] typeArgs = { queueItemType };
            var queueType = typeof(PersistentQueue<>).MakeGenericType(typeArgs);
            return Activator.CreateInstance(queueType, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { queueInstance.QueueInstanceId, queueInstance.Settings }, null) as IPersistentQueue;
        }
    }
}