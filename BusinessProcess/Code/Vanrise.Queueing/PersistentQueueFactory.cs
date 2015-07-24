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

        #endregion
        public PersistentQueue<T> GetQueue<T>(string queueName) where T : PersistentQueueItem
        {
            IQueueDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            QueueInstance queueInstance = dataManager.GetQueueInstance(queueName);
            if (queueInstance == null || queueInstance.Status != QueueInstanceStatus.ReadyToUse)
                return null;

            Type queueItemType = Type.GetType(queueInstance.ItemFQTN);
            if (typeof(T) != queueItemType)
                throw new Exception(String.Format("Queue '{0}' is of item type {1}. Expected Item Type {2}", queueName, queueItemType, typeof(T)));

            return new PersistentQueue<T>(queueInstance.QueueInstanceId, queueInstance.Settings);
        }

        public IPersistentQueue GetQueue(string queueName)
        {
            IQueueDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            QueueInstance queueInstance = dataManager.GetQueueInstance(queueName);
            if (queueInstance == null || queueInstance.Status != QueueInstanceStatus.ReadyToUse)
                return null;

            Type queueItemType = Type.GetType(queueInstance.ItemFQTN);
            Type[] typeArgs = { queueItemType };
            var queueType = typeof(PersistentQueue<>).MakeGenericType(typeArgs);
            return Activator.CreateInstance(queueType, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { queueInstance.QueueInstanceId, queueInstance.Settings }, null) as IPersistentQueue;
        }

        public bool QueueExists(string queueName)
        {
            IQueueDataManager dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            QueueInstance queueInstance = dataManagerQueue.GetQueueInstance(queueName);
            return queueInstance != null && queueInstance.Status == QueueInstanceStatus.ReadyToUse;
        }

        public void CreateQueue(string queueItemFQTN, string queueName, string queueTitle, IEnumerable<string> sourceQueueNames, QueueSettings queueSettings = null)
        {
            IQueueDataManager dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            IQueueItemDataManager dataManagerQueueItem = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();    
            Type queueItemType = Type.GetType(queueItemFQTN);
            if(queueItemType == null)
                throw new Exception(String.Format("type '{0}' is not available", queueItemFQTN));
            
            PersistentQueueItem emptyInstance = Activator.CreateInstance(queueItemType) as PersistentQueueItem;
            if (emptyInstance == null)
                throw new Exception(String.Format("{0} is not of type PersistentQueueItem", queueItemFQTN));
            List<int> sourceQueueIds = new List<int>();
            if (sourceQueueNames != null)
            {
                foreach (var sourceQueueName in sourceQueueNames)
                {
                    QueueInstance sourceQueue = dataManagerQueue.GetQueueInstance(sourceQueueName);
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
            Action createQueueAction = () =>
            {
                int queueId = dataManagerQueue.InsertQueueInstance(queueName, queueTitle, QueueInstanceStatus.New, itemTypeId, queueSettings);
                dataManagerQueue.InsertSubscription(sourceQueueIds, queueId);
                dataManagerQueueItem.CreateQueue(queueId);
                dataManagerQueueItem.InsertQueueItemIDGen(queueId);
                dataManagerQueue.UpdateQueueInstanceStatus(queueName, QueueInstanceStatus.ReadyToUse);
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

        public void CreateQueue<T>(string queueName, string queueTitle, IEnumerable<string> sourceQueueNames, QueueSettings queueSettings = null) where T : PersistentQueueItem
        {
            CreateQueue(typeof(T).AssemblyQualifiedName, queueName, queueTitle, sourceQueueNames, queueSettings);
        }

        public void CreateQueue<T>(string queueName, string queueTitle, QueueSettings queueSettings = null) where T : PersistentQueueItem
        {
            CreateQueue<T>(queueName, queueTitle, null, queueSettings);
        }

        public void CreateQueue<T>(string queueName, QueueSettings queueSettings = null) where T : PersistentQueueItem
        {
            CreateQueue<T>(queueName, queueName, null, queueSettings);
        }

        public void CreateQueueIfNotExists(string queueItemFQTN, string queueName, string queueTitle = null, IEnumerable<string> sourceQueueNames = null, QueueSettings queueSettings = null)
        {
            if (!QueueExists(queueName))
            {
                try
                {
                    CreateQueue(queueItemFQTN, queueName, queueTitle ?? queueName, sourceQueueNames, queueSettings);
                }
                catch
                {
                    if (!QueueExists(queueName))
                        throw;
                }
            }
        }

        public void CreateQueueIfNotExists<T>(string queueName, string queueTitle = null, IEnumerable<string> sourceQueueNames = null, QueueSettings queueSettings = null) where T : PersistentQueueItem
        {      
            CreateQueueIfNotExists(typeof(T).AssemblyQualifiedName, queueName, queueTitle, sourceQueueNames, queueSettings);
        }
    }
}