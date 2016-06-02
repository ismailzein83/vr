using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class ZoneDataManager :BaseSQLDataManager, IZoneDataManager
    {
           
        #region ctor/Local Variables
        public ZoneDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "Retail_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods

        public bool AreZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail.Zone", ref updateHandle);
        }
        public List<Zone> GetZones()
        {
            return GetItemsSP("Retail.sp_Zone_GetAll", ZoneMapper);
        }
        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
        private Zone ZoneMapper(IDataReader reader)
        {
           return new Zone
            {
                ZoneId = (int)reader["ID"],
                Name = reader["Name"] as string,
                CountryId = (int)reader["CountryId"],
            };
        }

        #endregion
    }
}
