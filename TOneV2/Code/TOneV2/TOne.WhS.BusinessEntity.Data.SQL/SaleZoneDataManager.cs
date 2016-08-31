using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleZoneDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISaleZoneDataManager
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
            return GetItemsSP("TOneWhS_BE.sp_SaleZone_GetAll", SaleZoneMapper);
        }
        public List<SaleZone> GetSaleZones(int sellingNumberPlanId)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZone_GetByNumberPlan", SaleZoneMapper, sellingNumberPlanId);
        }
        public List<SaleZoneInfo> GetSaleZonesInfo(int sellingNumberPlanId, string filter)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZoneInfo_GetFiltered", SaleZoneInfoMapper, sellingNumberPlanId, filter);
        }
        public bool AreZonesUpdated(ref object lastReceivedDataInfo)
        {
            return IsDataUpdated("TOneWhS_BE.SaleZone", ref lastReceivedDataInfo);
        }
        public IEnumerable<long> GetSaleZoneIds(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            List<long> saleZoneIds = new List<long>();
            ExecuteReaderSP("[TOneWhS_BE].[sp_SaleZone_GetIds]", (reader) =>
            {
                while (reader.Read())
                {
                    long saleZoneId = GetReaderValue<Int64>(reader, "Id");
                    saleZoneIds.Add(saleZoneId);
                }
            }, effectiveOn, isEffectiveInFuture);
            return saleZoneIds;
        }

        #endregion

        #region Private Methods
        #endregion

        #region Mappers

        SaleZone SaleZoneMapper(IDataReader reader)
        {
            SaleZone saleZone = new SaleZone();

            saleZone.SaleZoneId = (long)reader["ID"];
            saleZone.SellingNumberPlanId = (int)reader["SellingNumberPlanID"];
            saleZone.CountryId = GetReaderValue<int>(reader, "CountryID");
            saleZone.Name = reader["Name"] as string;
            saleZone.BED = GetReaderValue<DateTime>(reader, "BED");
            saleZone.EED = GetReaderValue<DateTime?>(reader, "EED");
            saleZone.SourceId = reader["SourceID"] as string;
            return saleZone;
        }

        SaleZoneInfo SaleZoneInfoMapper(IDataReader reader)
        {
            SaleZoneInfo saleZoneInfo = new SaleZoneInfo
            {
                SaleZoneId = (long)reader["ID"],
                Name = reader["Name"] as string,
                SellingNumberPlanId =  (int)reader["SellingNumberPlanID"]
                
            };
            return saleZoneInfo;
        }
        #endregion
  
    }
}
