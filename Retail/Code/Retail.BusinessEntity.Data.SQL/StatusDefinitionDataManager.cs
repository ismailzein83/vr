using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Retail.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    class StatusDefinitionDataManager: BaseSQLDataManager, IStatusDefinitionDataManager
    {
        #region ctor/Local Variables
        public StatusDefinitionDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<StatusDefinition> GetStatusDefinition()
        {
            return GetItemsSP("Retail.sp_StatusDefinition_GetAll", StatusDefinitionMapper);
        }

        public bool AreStatusDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.StatusDefinition", ref updateHandle);
        }

        public bool Insert(StatusDefinition statusDefinitionItem)
        {
            //object statusDefinitionId;
            //string serializedSettings = switchItem.Settings != null ? Vanrise.Common.Serializer.Serialize(switchItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_StatusDefinition_Insert", statusDefinitionItem.StatusDefinitionId, statusDefinitionItem.Name);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(StatusDefinition statusDefinitionItem)
        {
            //string serializedSettings = switchItem.Settings != null ? Vanrise.Common.Serializer.Serialize(switchItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_StatusDefinition_Update", statusDefinitionItem.StatusDefinitionId, statusDefinitionItem.Name);
            return (affectedRecords > 0);
        }

        #endregion

        #region Mappers
        StatusDefinition StatusDefinitionMapper(IDataReader reader)
        {
            StatusDefinition statusDefinition = new StatusDefinition
            {
                StatusDefinitionId = (Guid) reader["ID"],
                Name = reader["Name"] as string,
                Settings = null,
                //Settings = Vanrise.Common.Serializer.Deserialize<ServiceTypeSettings>(reader["Settings"] as string),
            }; 
            return statusDefinition;
        }
        #endregion
    }
}
