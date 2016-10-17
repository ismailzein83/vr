using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleRateDataManager : BaseSQLDataManager, ISaleRateDataManager
    {

        #region ctor/Local Variables
        public SaleRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }

        #endregion

        #region Public Methods

        public SaleRate GetSaleRateById(long rateId)
        {
            return GetItemSP("TOneWhS_BE.sp_SaleRate_GetByID", SaleRateMapper, rateId);
        }
        public IEnumerable<SaleRate> GetFilteredSaleRates(SaleRateQuery query)
        {
            string zonesids = null;
            if (query.ZonesIds != null && query.ZonesIds.Count() > 0)
                zonesids = string.Join<long>(",", query.ZonesIds);

            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetFiltered", SaleRateMapper, query.EffectiveOn, query.SellingNumberPlanId, zonesids, query.OwnerType, query.OwnerId);
        }

        public List<SaleRate> GetEffectiveSaleRates(DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetByEffective", SaleRateMapper, effectiveOn);
        }

        public List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetEffectiveAfter", SaleRateMapper, sellingNumberPlanId, minimumDate);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetEffectiveAfterByOwner", SaleRateMapper, ownerType, ownerId, minimumDate);
        }

        public List<SaleRate> GetSaleRatesInBetweenPeriod(DateTime fromTime, DateTime tillTime)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetBetweenPeriod", SaleRateMapper, fromTime, tillTime);
        }

        public List<SaleRate> GetEffectiveSaleRateByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DataTable saleRateOwners = BuildRoutingOwnerInfoTable(customerInfos);
            return GetItemsSPCmd("[TOneWhS_BE].[sp_SaleRate_GetEffectiveByOwner]", SaleRateMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@SaleRateOwner", SqlDbType.Structured);
                dtPrm.Value = saleRateOwners;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }
        public bool AreSaleRatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleRate", ref updateHandle);
        }
        public IEnumerable<SaleRate> GetExistingRatesByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED)
        {
            string zoneIdsParameter = string.Join(",", zoneIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetExistingByZoneIDs", SaleRateMapper, ownerType, ownerId, zoneIdsParameter, minEED);
        }

        internal static DataTable BuildRoutingOwnerInfoTable(IEnumerable<RoutingCustomerInfoDetails> customerInfos)
        {
            HashSet<int> addedSellingProductIds = new HashSet<int>();
            DataTable dtRoutingInfos = GetRoutingOwnerInfoTable();
            dtRoutingInfos.BeginLoadData();
            foreach (var c in customerInfos)
            {
                DataRow drCustomer = dtRoutingInfos.NewRow();
                drCustomer["OwnerId"] = c.CustomerId;
                drCustomer["OwnerTpe"] = (int)SalePriceListOwnerType.Customer;
                dtRoutingInfos.Rows.Add(drCustomer);

                if (addedSellingProductIds.Contains(c.SellingProductId))
                    continue;

                DataRow drSP = dtRoutingInfos.NewRow();
                drSP["OwnerId"] = c.SellingProductId;
                drSP["OwnerTpe"] = (int)SalePriceListOwnerType.SellingProduct;
                dtRoutingInfos.Rows.Add(drSP);
                addedSellingProductIds.Add(c.SellingProductId);
            }
            dtRoutingInfos.EndLoadData();
            return dtRoutingInfos;
        }
        private static DataTable GetRoutingOwnerInfoTable()
        {
            DataTable dtRoutingInfos = new DataTable();
            dtRoutingInfos.Columns.Add("OwnerId", typeof(Int32));
            dtRoutingInfos.Columns.Add("OwnerTpe", typeof(Int32));
            return dtRoutingInfos;
        }

        public IEnumerable<SaleRate> GetFutureSaleRates(SalePriceListOwnerType ownerType, int ownerId)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetFutureByOwner", SaleRateMapper, ownerType, ownerId);
        }

        #endregion

        #region Mappers

        private SaleRate SaleRateMapper(IDataReader reader)
        {
            SaleRate saleRate = new SaleRate();

            saleRate.SaleRateId = (long)reader["ID"];
            saleRate.ZoneId = (long)reader["ZoneID"];
            saleRate.PriceListId = (int)reader["PriceListID"];

            saleRate.RateTypeId = GetReaderValue<int?>(reader, "RateTypeID");
            saleRate.NormalRate = (decimal)reader["Rate"];

            saleRate.BED = (DateTime)reader["BED"];
            saleRate.EED = GetReaderValue<DateTime?>(reader, "EED");
            
            saleRate.RateChange = (Entities.RateChangeType)GetReaderValue<byte>(reader, "Change");
            saleRate.CurrencyId = GetReaderValue<int?>(reader, "CurrencyId");
            return saleRate;
        }

        #endregion
    }
}
