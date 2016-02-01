﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class GenericRuleDefinitionDataManager : BaseSQLDataManager, IGenericRuleDefinitionDataManager
    {
        public GenericRuleDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods
        
        public IEnumerable<GenericRuleDefinition> GetGenericRuleDefinitions()
        {
            return GetItemsSP("genericdata.sp_GenericRuleDefinition_GetAll", GenericRuleDefinitionMapper);
        }

        public bool AreGenericRuleDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.GenericRuleDefinition", ref updateHandle);
        }

        public bool AddGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition, out int insertedId)
        {
            object genericRuleDefinitionId;

            int affectedRows = ExecuteNonQuerySP("genericdata.sp_GenericRuleDefinition_Insert", out genericRuleDefinitionId, genericRuleDefinition.Name, Vanrise.Common.Serializer.Serialize(genericRuleDefinition));
            insertedId = (affectedRows == 1) ? (int)genericRuleDefinitionId : -1;

            return (affectedRows == 1);
        }

        public bool UpdateGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            int affectedRows = ExecuteNonQuerySP("genericdata.sp_GenericRuleDefinition_Update", genericRuleDefinition.GenericRuleDefinitionId, genericRuleDefinition.Name, Vanrise.Common.Serializer.Serialize(genericRuleDefinition));
            return (affectedRows == 1);
        }

        #endregion

        #region Mappers

        GenericRuleDefinition GenericRuleDefinitionMapper(IDataReader reader)
        {
            GenericRuleDefinition genericRuleDefinition = Vanrise.Common.Serializer.Deserialize<GenericRuleDefinition>((string)reader["Details"]);
            return genericRuleDefinition;
        }

        #endregion
    }
}
