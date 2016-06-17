﻿using System;
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
        public IEnumerable<SaleRate> GetFilteredSaleRates(SaleRateQuery query)
        {
            string zonesids = null;
            if (query.ZonesIds != null && query.ZonesIds.Count() > 0)
                zonesids = string.Join<int>(",", query.ZonesIds);

            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetFiltered", SaleRateMapper, query.EffectiveOn, query.SellingNumberPlanId, zonesids, query.OwnerType, query.OwnerId);
        }

        public List<SaleRate> GetEffectiveSaleRates(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetByOwnerAndEffective", SaleRateMapper, ownerType, ownerId, effectiveOn);
        }

        public List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetEffectiveAfter", SaleRateMapper, sellingNumberPlanId, minimumDate);
        }

        public List<SaleRate> GetSaleRatesInBetweenPeriod(DateTime fromTime, DateTime tillTime)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetBetweenPeriod", SaleRateMapper, fromTime, tillTime);
        }
        public List<SaleRate> GetEffectiveSaleRateByCustomers(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DataTable dtActiveCustomers = CarrierAccountDataManager.BuildRoutingCustomerInfoTable(customerInfos);
            return GetItemsSPCmd("[TOneWhS_BE].[sp_SaleRate_GetEffectiveByCustomers]", SaleRateMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ActiveCustomersInfo", SqlDbType.Structured);
                dtPrm.Value = dtActiveCustomers;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@CustomerOwnerType", SalePriceListOwnerType.Customer));
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
        #endregion

        #region Mappers

        private SaleRate SaleRateMapper(IDataReader reader)
        {
            SaleRate saleRate = new SaleRate();

            saleRate.SaleRateId = (long)reader["ID"];
            saleRate.ZoneId = (long)reader["ZoneID"];
            saleRate.PriceListId = (int)reader["PriceListID"];

            saleRate.NormalRate = (decimal)reader["Rate"];
            saleRate.OtherRates = reader["OtherRates"] as string != null ? Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal>>(reader["OtherRates"] as string) : null;

            saleRate.BED = (DateTime)reader["BED"];
            saleRate.EED = GetReaderValue<DateTime?>(reader, "EED");
            // saleRate.RateChange = GetReaderValue<Entities.RateChangeType>(reader, "change");
            return saleRate;
        }

        #endregion
    }
}
