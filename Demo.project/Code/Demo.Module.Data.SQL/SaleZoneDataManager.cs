using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class SaleZoneDataManager : BaseSQLDataManager, ISaleZoneDataManager
    {

        #region ctor/Local Variables
        public SaleZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public IEnumerable<SaleZone> GetAllSaleZones()
        {
            return GetItemsSP("[dbo].[sp_Zone_GetAll]", SaleZoneMapper);
        }
        public List<SaleZone> GetSaleZones()
        {
            return GetItemsSP("[dbo].[sp_Zone_GetAll]", SaleZoneMapper);
        }
        public List<SaleZoneInfo> GetSaleZonesInfo(string filter)
        {
            return GetItemsSP("[dbo].[sp_ZoneInfo_GetFiltered]", SaleZoneInfoMapper, filter);
        }
        public bool AreZonesUpdated(ref object lastReceivedDataInfo)
        {
            return IsDataUpdated("[dbo].Zone", ref lastReceivedDataInfo);
        }
        public IEnumerable<long> GetSaleZoneIds(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            List<long> saleZoneIds = new List<long>();
            ExecuteReaderSP("[dbo].[sp_Zone_GetIds]", (reader) =>
            {
                while (reader.Read())
                {
                    long saleZoneId = GetReaderValue<Int64>(reader, "Id");
                    saleZoneIds.Add(saleZoneId);
                }
            }, effectiveOn, isEffectiveInFuture);
            return saleZoneIds;
        }
        public List<SaleZone> GetSaleZonesEffectiveAfter(int countryId, DateTime minimumDate)
        {
            return GetItemsSP("[dbo].[sp_Zone_GetByDate]", SaleZoneMapper, countryId, minimumDate);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Mappers

        SaleZone SaleZoneMapper(IDataReader reader)
        {
            SaleZone sellingNumberPlan = new SaleZone();

            sellingNumberPlan.SaleZoneId = (long)reader["ID"];
            sellingNumberPlan.CountryId = GetReaderValue<int>(reader, "CountryID");
            sellingNumberPlan.Name = reader["Name"] as string;
            sellingNumberPlan.BED = GetReaderValue<DateTime>(reader, "BED");
            sellingNumberPlan.EED = GetReaderValue<DateTime?>(reader, "EED");

            return sellingNumberPlan;
        }

        SaleZoneInfo SaleZoneInfoMapper(IDataReader reader)
        {
            SaleZoneInfo saleZoneInfo = new SaleZoneInfo
            {
                SaleZoneId = (long)reader["ID"],
                Name = reader["Name"] as string
                
            };
            return saleZoneInfo;
        }
        #endregion
  
    }
}
