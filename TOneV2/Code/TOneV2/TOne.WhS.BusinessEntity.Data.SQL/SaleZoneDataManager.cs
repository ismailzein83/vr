using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleZoneDataManager : BaseTOneDataManager, ISaleZoneDataManager
    {
        public SaleZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

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

        #region Mappers

        SaleZone SaleZoneMapper(IDataReader reader)
        {
            SaleZone sellingNumberPlan = new SaleZone();

            sellingNumberPlan.SaleZoneId = (long)reader["ID"];
            sellingNumberPlan.SellingNumberPlanId = (int)reader["SellingNumberPlanID"];
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
                Name = reader["Name"] as string,
            };
            return saleZoneInfo;
        }
        #endregion


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
    }
}
