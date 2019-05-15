﻿using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPDefinitionDataManager : BaseSQLDataManager, IBPDefinitionDataManager
    {
        public BPDefinitionDataManager()
            : base(GetConnectionStringName("BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        #region Public Methods

        public List<BPDefinition> GetBPDefinitions()
        {
            return GetItemsSP("bp.sp_BPDefinition_GetAll", BPDefinitionMapper);
        }

        public bool InsertBPDefinition(BPDefinition bpDefinition)
        {
            string serializedConfiguration = bpDefinition.Configuration != null ? Vanrise.Common.Serializer.Serialize(bpDefinition.Configuration) : null;
            int affectedRecords = ExecuteNonQuerySP("[bp].[sp_BPDefinition_Insert]", bpDefinition.BPDefinitionID, bpDefinition.Name, bpDefinition.Title, bpDefinition.VRWorkflowId, serializedConfiguration);
            return affectedRecords > 0 ? true : false;
        }

        public bool UpdateBPDefinition(BPDefinition bpDefinition)
        {
            int recordesEffected = ExecuteNonQuerySP("[bp].[sp_BPDefinition_Update]", bpDefinition.BPDefinitionID, bpDefinition.Title, bpDefinition.VRWorkflowId, Vanrise.Common.Serializer.Serialize(bpDefinition.Configuration));
            return (recordesEffected > 0);
        }

        public bool AreBPDefinitionsUpdated(ref object lastReceivedDataInfo)
        {
            return base.IsDataUpdated("[bp].[BPDefinition]", ref lastReceivedDataInfo);
        }

        #endregion

        #region Mappers

        BPDefinition BPDefinitionMapper(IDataReader reader)
        {
            var bpDefinition = new BPDefinition
            {
                BPDefinitionID = GetReaderValue<Guid>(reader, "ID"),
                DevProjectId = GetReaderValue<Guid?>(reader, "DevProjectID"),
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                VRWorkflowId = GetReaderValue<Guid?>(reader, "VRWorkflowId")
            };

            string workflowTypeAsString = reader["FQTN"] as string;
            if (!string.IsNullOrEmpty(workflowTypeAsString))
            {
                try
                {
                    bpDefinition.WorkflowType = Type.GetType(workflowTypeAsString);
                }
                catch (Exception ex)
                {

                }
            }

            string config = reader["Config"] as string;
            if (!String.IsNullOrWhiteSpace(config))
                bpDefinition.Configuration = Serializer.Deserialize<BPConfiguration>(config);

            return bpDefinition;
        }
        #endregion
    }
}