using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using System;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPBusinessRuleDefinitionDataManager : BaseSQLDataManager, IBPBusinessRuleDefinitionDataManager
    {
        #region Fields / Constructors

        public BPBusinessRuleDefinitionDataManager()
            : base(GetConnectionStringName("BusinessProcessConfigDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<BPBusinessRuleDefinition> GetBPBusinessRuleDefinitions()
        {
            return GetItemsSP("bp.sp_BPBusinessRuleDefinition_GetAll", BPBusinessRuleDefintionMapper);
        }

        public bool AreBPBusinessRuleDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[bp].[BPBusinessRuleDefinition]", ref updateHandle);
        }

        #endregion

        #region Mappers

        BPBusinessRuleDefinition BPBusinessRuleDefintionMapper(IDataReader reader)
        {
            var businessRuleDefinition = new BPBusinessRuleDefinition
            {
                BPBusinessRuleDefinitionId = GetReaderValue<Guid>(reader, "ID"),
                Settings = Serializer.Deserialize<BPBusinessRuleSettings>(reader["Settings"] as string),
                BPDefintionId = GetReaderValue<Guid>(reader, "BPDefintionId"),
                Name = reader["Name"] as string,
                Rank = (int)reader["Rank"]
            };

            return businessRuleDefinition;
        }

        #endregion
    }
}