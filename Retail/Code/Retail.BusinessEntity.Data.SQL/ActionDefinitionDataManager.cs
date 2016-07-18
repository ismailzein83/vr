using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class ActionDefinitionDataManager:BaseSQLDataManager,IActionDefinitionDataManager
    {
       
        #region Constructors
        public ActionDefinitionDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "Retail_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<ActionDefinition> GetActionDefinitions()
        {
            return GetItemsSP("Retail_BE.sp_ActionDefinition_GetAll", ActionDefinitionMapper);
        }

        public bool AreActionDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.ActionDefinition", ref updateHandle);
        }

        public bool Insert(ActionDefinition actionDefinition)
        {
            string serializedSettings = actionDefinition.Settings != null ? Vanrise.Common.Serializer.Serialize(actionDefinition.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail_BE.sp_ActionDefinition_Insert", actionDefinition.ActionDefinitionId, actionDefinition.Name, serializedSettings, actionDefinition.EntityType);
            return affectedRecords > 0;
        }

        public bool Update(ActionDefinition actionDefinition)
        {
            string serializedSettings = actionDefinition.Settings != null ? Vanrise.Common.Serializer.Serialize(actionDefinition.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail_BE.sp_ActionDefinition_Update", actionDefinition.ActionDefinitionId, actionDefinition.Name, serializedSettings,actionDefinition.EntityType);
            return (affectedRecords > 0);
        }

        #endregion

        #region  Mappers

        private ActionDefinition ActionDefinitionMapper(IDataReader reader)
        {
            return new ActionDefinition()
            {
                ActionDefinitionId = GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<ActionDefinitionSettings>(reader["Settings"] as string),
                EntityType = GetReaderValue<EntityType>(reader, "EntityType"),
            };
        }

        #endregion
    }
}
