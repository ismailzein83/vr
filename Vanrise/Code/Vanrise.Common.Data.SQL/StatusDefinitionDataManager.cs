using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class StatusDefinitionDataManager : BaseSQLDataManager, IStatusDefinitionDataManager
    {

        #region ctor/Local Variables
        public StatusDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #endregion

        #region Public Methods
        public List<StatusDefinition> GetStatusDefinition()
        {
            return GetItemsSP("Common.sp_StatusDefinition_GetAll", StatusDefinitionMapper);
        }
        public bool AreStatusDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Common.StatusDefinition", ref updateHandle);
        }
        public bool Insert(StatusDefinition statusDefinitionItem)
        {
            string serializedSettings = statusDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(statusDefinitionItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Common.sp_StatusDefinition_Insert", statusDefinitionItem.StatusDefinitionId, statusDefinitionItem.Name, statusDefinitionItem.BusinessEntityDefinitionId,serializedSettings, statusDefinitionItem.CreatedBy, statusDefinitionItem.LastModifiedBy);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }
        public bool Update(StatusDefinition statusDefinitionItem)
        {
            string serializedSettings = statusDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(statusDefinitionItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Common.sp_StatusDefinition_Update", statusDefinitionItem.StatusDefinitionId, statusDefinitionItem.Name, statusDefinitionItem.BusinessEntityDefinitionId,serializedSettings, statusDefinitionItem.LastModifiedBy);
            return (affectedRecords > 0);
        }
        #endregion

        #region Mappers
        StatusDefinition StatusDefinitionMapper(IDataReader reader)
        {
            StatusDefinition statusDefinition = new StatusDefinition
            {
                StatusDefinitionId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                BusinessEntityDefinitionId = GetReaderValue<Guid>(reader, "BusinessEntityDefinitionID"),
                Settings = reader["Settings"] as string != null ? Vanrise.Common.Serializer.Deserialize<StatusDefinitionSettings>(reader["Settings"] as string) : null,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime"),
            };
            return statusDefinition;
        }

        #endregion
    }
}
