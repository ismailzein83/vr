using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueDataManager : BaseSQLDataManager, IQueueDataManager
    {
        public QueueDataManager()
            : base(GetConnectionStringName("QueueDBConnStringKey", "QueueDBConnString"))
        {
        }

        //public void CreateQueue(string queueName, string title, string itemFQTN, QueueSettings settings, IEnumerable<int> sourceQueueIds)
        //{
        //    DataTable dtSourceQueueIds = new DataTable();
        //    dtSourceQueueIds.Columns.Add("ID", typeof(int));
        //    dtSourceQueueIds.BeginLoadData();
        //    if(sourceQueueIds != null)
        //    {
        //        foreach (var sourceQueueId in sourceQueueIds)
        //            dtSourceQueueIds.Rows.Add(sourceQueueId);
        //    }
        //    dtSourceQueueIds.EndLoadData();
        //    ExecuteNonQuerySPCmd("queue.sp_Queue_Create", (cmd) =>
        //        {
        //            cmd.Parameters.Add(new SqlParameter("@Name", queueName));
        //            cmd.Parameters.Add(new SqlParameter("@Title", title));
        //            cmd.Parameters.Add(new SqlParameter("@ItemFQTN", itemFQTN));
        //            cmd.Parameters.Add(new SqlParameter("@Settings", Serializer.Serialize(settings)));
        //            SqlParameter prmSourceQueueIds = new SqlParameter("@SourceQueueIDs", SqlDbType.Structured);
        //            prmSourceQueueIds.Value = dtSourceQueueIds;
        //            cmd.Parameters.Add(prmSourceQueueIds);
        //        });
        //}

        public int InsertOrUpdateQueueItemType(string itemFQTN, string title, QueueSettings defaultQueueSettings )
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

        public QueueInstance GetQueueInstance(string queueName)
        {
            return GetItemSP("queue.sp_QueueInstance_GetByName", QueueInstanceReader, queueName);
        }

        public List<QueueInstance> GetQueueInstances(IEnumerable<int> queueIds)
        {
            DataTable dtIds = new DataTable();
            dtIds.Columns.Add("ID");
            dtIds.BeginLoadData();
            foreach (var id in queueIds)
                dtIds.Rows.Add(id);
            dtIds.EndLoadData();
            return GetItemsSPCmd("queue.sp_QueueInstance_GetByIDs", QueueInstanceReader, (cmd) =>
                {
                    var prmIds = new SqlParameter("@IDs", SqlDbType.Structured);
                    prmIds.Value = dtIds;
                    cmd.Parameters.Add(prmIds);
                });
        }

        public List<QueueSubscription> GetSubscriptions()
        {
            return GetItemsSP("queue.sp_QueueSubscription_GetAll", QueueSubscriptionMapper, (int)QueueInstanceStatus.ReadyToUse);
        }

        public object GetSubscriptionsMaxTimestamp()
        {
            return ExecuteScalarSP("queue.sp_QueueSubscription_GetMaxTimestamp");
        }

        public bool HaveSubscriptionsChanged(object timestampToCompare)
        {
            if (timestampToCompare == null)
                return true;
            return !StructuralComparisons.StructuralEqualityComparer.Equals(timestampToCompare, GetSubscriptionsMaxTimestamp());
            //if (timestampToCompare == null)
            //    timestampToCompare = DBNull.Value;
            //return Convert.ToBoolean(ExecuteScalarSP("queue.sp_QueueSubscription_HasNewTimestamp", timestampToCompare));
        }

        public List<QueueItemType> GetQueueItemTypes()
        {
            return GetItemsSP("queue.sp_QueueItemTypes_GetAll", QueueItemTypeMapper);
        }

        #region Private Methods

        private QueueInstance QueueInstanceReader(IDataReader reader)
        {
            return new QueueInstance
            {
                QueueInstanceId = (int)reader["ID"],
                Name = reader["Name"] as string,
                ExecutionFlowId = GetReaderValue<int?>(reader, "ExecutionFlowID"),
                Title = reader["Title"] as string,
                Status = (QueueInstanceStatus)reader["Status"],
                ItemTypeId = (int)reader["ItemTypeID"],
                ItemFQTN = reader["ItemFQTN"] as string,
                Settings = Serializer.Deserialize<QueueSettings>(reader["Settings"] as string),
                CreateTime = GetReaderValue<DateTime>(reader, "CreatedTime")
            };
        }

        private QueueInstance QueueInstanceMapper(IDataReader reader)
        {
            return new QueueInstance
            {
                QueueInstanceId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                Status = (QueueInstanceStatus)reader["Status"],
                ItemTypeId = (int)reader["ItemTypeID"],
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

        public List<QueueInstance> GetQueueInstancesByTypes(IEnumerable<int> queueItemTypes)
        {
            return GetItemsSP("queue.sp_QueueInstance_GetByTypes", QueueInstanceMapper, queueItemTypes == null ? null : string.Join(",", queueItemTypes.Select(n => ((int)n).ToString()).ToArray()));
        }
    }
}
