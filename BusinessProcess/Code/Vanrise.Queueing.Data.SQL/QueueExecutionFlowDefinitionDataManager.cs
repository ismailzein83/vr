﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueExecutionFlowDefinitionDataManager : BaseSQLDataManager, IQueueExecutionFlowDefinitionDataManager
    {
        public QueueExecutionFlowDefinitionDataManager()
            : base(GetConnectionStringName("QueueDBConnStringKey", "QueueDBConnString"))
        {
        }

        public List<Entities.QueueExecutionFlowDefinition> GetAll()
        {
            return GetItemsSP("queue.sp_ExecutionFlowDefinition_GetAll", ExecutionFlowDefinitionMapper);
        }


       
        public bool AreQueueExecutionFlowDefinitionUpdated(ref object updateHandle) {
            return base.IsDataUpdated("queue.[ExecutionFlowDefinition]", ref updateHandle);
        }


        public bool UpdateExecutionFlowDefinition(QueueExecutionFlowDefinition executionFlowDefinitionObject)
        {
            string serializedObj = null;
            if (executionFlowDefinitionObject.Stages != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(executionFlowDefinitionObject.Stages);
            }
            int recordesEffected = ExecuteNonQuerySP("queue.sp_ExecutionFlowDefinition_Update", executionFlowDefinitionObject.ID, executionFlowDefinitionObject.Name, executionFlowDefinitionObject.Title, serializedObj);
            return (recordesEffected > 0);
        }


        public bool AddExecutionFlowDefinition(QueueExecutionFlowDefinition executionFlowDefinition, out int insertedId)
        {
            object executionFlowDefinitionID;
            string serializedObj = null;
            if (executionFlowDefinition.Stages != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(executionFlowDefinition.Stages);
            }
            int recordesEffected = ExecuteNonQuerySP("queue.sp_ExecutionFlowDefinition_Insert", out executionFlowDefinitionID, executionFlowDefinition.Name, executionFlowDefinition.Title, serializedObj);

            insertedId = (recordesEffected > 0) ? (int)executionFlowDefinitionID : -1;
            return (recordesEffected > 0);
        }

        private Entities.QueueExecutionFlowDefinition ExecutionFlowDefinitionMapper(IDataReader reader)
        {
            return new Entities.QueueExecutionFlowDefinition
            {
                ID = (int)reader["ID"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                Stages=Vanrise.Common.Serializer.Deserialize<List<QueueExecutionFlowStage>>(reader["Stages"] as string),
            };
        }
    }
}
