using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueDataManager : BaseSQLDataManager, IQueueDataManager
    {
        public QueueDataManager()
            : base(GetConnectionStringName("QueueingConfigDBConnStringKey", "QueueingConfigDBConnString"))
        {
        }

       
        public int InsertOrUpdateQueueItemType(string itemFQTN, string title, QueueSettings defaultQueueSettings)
        {
            object id;
            ExecuteNonQuerySP("queue.sp_QueueItemType_InsertOrUpdate", out id, itemFQTN, title, Serializer.Serialize(defaultQueueSettings));
            return (int)id;
        }

        public int InsertQueueInstance(int executionFlowId, string stageName, string queueName, string title, QueueInstanceStatus status, int itemTypeId, QueueSettings settings)
        {
            object id;
            ExecuteNonQuerySP("queue.sp_QueueInstance_Insert", out id, executionFlowId, stageName, queueName, title, (int)status, itemTypeId, settings != null ? (object)Serializer.Serialize(settings) : DBNull.Value);
            return (int)id;
        }

        public bool UpdateQueueInstance(string queueName, string stageName, string title, QueueSettings settings)
        {
            return ExecuteNonQuerySP("queue.sp_QueueInstance_Update", queueName, stageName, title, settings != null ? (object)Serializer.Serialize(settings) : DBNull.Value) > 0;
        }

        public void UpdateQueueInstanceStatus(string queueName, QueueInstanceStatus status)
        {
            ExecuteNonQuerySP("queue.sp_QueueInstance_UpdateStatus", queueName, (int)status);
        }

        public bool UpdateQueueName(string queueName, QueueInstanceStatus status, string newQueueName)
        {
            return ExecuteNonQuerySP("queue.sp_QueueInstance_UpdateName", queueName, (int)status, newQueueName) > 0;
        }

        public List<QueueInstance> GetAllQueueInstances()
        {
            return GetItemsSP("queue.sp_QueueInstance_GetAll", QueueInstanceMapper);
        }

        public List<QueueItemType> GetQueueItemTypes()
        {
            return GetItemsSP("queue.sp_QueueItemTypes_GetAll", QueueItemTypeMapper);
        }

        #region Private Methods


        private QueueInstance QueueInstanceMapper(IDataReader reader)
        {
            return new QueueInstance
            {
                QueueInstanceId = (int)reader["ID"],
                Name = reader["Name"] as string,
                ExecutionFlowId = GetReaderValue<int?>(reader, "ExecutionFlowID"),
                StageName = reader["StageName"] as string,
                Title = reader["Title"] as string,
                Status = (QueueInstanceStatus)reader["Status"],
                ItemTypeId = (int)reader["ItemTypeID"],
                ItemFQTN = reader["ItemFQTN"] as string,
                Settings = Serializer.Deserialize<QueueSettings>(reader["Settings"] as string),
                CreateTime = GetReaderValue<DateTime>(reader, "CreatedTime")
            };
        }


        private QueueSubscription QueueSubscriptionMapper(IDataReader reader)
        {
            return new QueueSubscription
            {
                QueueID = (int)reader["QueueID"],
                SubsribedQueueID = (int)reader["SubscribedQueueID"]
            };
        }

        private QueueItemType QueueItemTypeMapper(IDataReader reader)
        {
            return new QueueItemType
            {
                Id = (int)reader["Id"],
                ItemFQTN = reader["ItemFQTN"] as string,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                DefaultQueueSettings = reader["DefaultQueueSettings"] as string,
                Title = reader["Title"] as string,
            };
        }

        #endregion


        public bool AreQueuesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("queue.QueueInstance", ref updateHandle);
        }
    }
}
