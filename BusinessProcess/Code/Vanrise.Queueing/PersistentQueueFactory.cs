﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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

            if (queueInstance.Status != QueueInstanceStatus.ReadyToUse)
                throw new Exception(String.Format("Queue '{0}' is not ready to use", queueName));

            Type queueItemType = Type.GetType(queueInstance.ItemFQTN);
            if (typeof(T) != queueItemType)
                throw new Exception(String.Format("Queue '{0}' is not of type {1}. Expected Item Type {2}", queueName, typeof(T), queueItemType));

            return new PersistentQueue<T>(queueInstance.QueueInstanceId, queueInstance.Settings);
        }

        public static bool IsQueueExists(string queueName)
        {
            IQueueDataManager dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            return dataManagerQueue.GetQueueInstance(queueName) != null;
        }

        public static void CreateQueue<T>(string queueName, string queueTitle, IEnumerable<string> sourceQueueNames, QueueSettings queueSettings = null) where T : PersistentQueueItem
        {
            IQueueDataManager dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            IQueueItemDataManager dataManagerQueueItem = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            string itemFQTN = typeof(T).AssemblyQualifiedName;
            T emptyInstance = Activator.CreateInstance<T>();
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

                    if (!sourceQueueIds.Contains(sourceQueue.QueueInstanceId))
                        sourceQueueIds.Add(sourceQueue.QueueInstanceId);
                }
            }
            int itemTypeId = dataManagerQueue.InsertOrUpdateQueueItemType(itemFQTN, emptyInstance.ItemTypeTitle, emptyInstance.DefaultQueueSettings);
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

        public static void CreateQueue<T>(string queueName, string queueTitle, QueueSettings queueSettings = null) where T : PersistentQueueItem
        {
            CreateQueue<T>(queueName, queueTitle, null, queueSettings);
        }

        public static void CreateQueue<T>(string queueName, QueueSettings queueSettings = null) where T : PersistentQueueItem
        {
            CreateQueue<T>(queueName, queueName, null, queueSettings);
        }
    }
}