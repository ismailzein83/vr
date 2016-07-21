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
