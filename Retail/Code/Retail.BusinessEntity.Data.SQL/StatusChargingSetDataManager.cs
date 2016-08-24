using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Retail.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;

namespace Retail.BusinessEntity.Data.SQL
{
    public class StatusChargingSetDataManager : BaseSQLDataManager, IStatusChargingSetDataManager
    {
        #region Public Methods
        public List<StatusChargingSet> GetStatusChargingSets()
        {
            return GetItemsSP("Retail.sp_StatusChargingSet_GetAll", StatusChargingSetsnMapper);
        }
        public bool Insert(StatusChargingSet statusChargingSetItem, out int insertedId)
        {
            string serializedSettings = statusChargingSetItem.Settings != null ? Serializer.Serialize(statusChargingSetItem.Settings) : null;

            object insertedID;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_StatusChargingSet_Insert",out insertedID, statusChargingSetItem.Name, serializedSettings);
            insertedId = (int) insertedID;
            return affectedRecords > 0;
        }
        public bool Update(StatusChargingSet statusChargingSet)
        {
            string serializedSettings = statusChargingSet.Settings != null ? Serializer.Serialize(statusChargingSet.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_StatusChargingSet_Update",
                statusChargingSet.StatusChargingSetId, statusChargingSet.Name, serializedSettings);
            return (affectedRecords > 0);
        }
        #endregion

        #region Mappers
        StatusChargingSet StatusChargingSetsnMapper(IDataReader reader)
        {
            StatusChargingSet statusChargingSet = new StatusChargingSet
            {
                StatusChargingSetId = (int)reader["ID"],
                Name = reader["Name"] as string
            };
            string settingsSerialized = reader["Settings"] as string;
            if (settingsSerialized != null)
                statusChargingSet.Settings = Serializer.Deserialize<StatusChargingSetSettings>(settingsSerialized);
            return statusChargingSet;
        }
        #endregion
    }
}
