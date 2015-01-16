using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace TABS.DAL
{
    public partial class ObjectAssembler
    {
        public static DataTable GetRouteOverrides()
        {
            string sql = QueryBuilder.GetRoutOverridesQuery();
            return DataHelper.GetDataTable(sql);
        }

        public static void UpdateZoneCodeGroup(int zoneid, string code)
        {
            string sql = QueryBuilder.UpdateZoneCodeGroupQuery(zoneid, code);
            TABS.DataHelper.ExecuteNonQuery(sql);
        }

        public static IDataReader GetZonesAndCodes(DateTime effectiveDate)
        {
            string sql = QueryBuilder.GetZonesAndCodesQuery(effectiveDate);
            return TABS.DataHelper.ExecuteReader(sql);
        }

        public static void InsertSwitchCarrierMappings()
        {
            string sql = QueryBuilder.InsertSwitchCarrierMappingsQuery();

            foreach (TABS.Switch definedSwitch in TABS.Switch.All.Values)
            {
                var manager = definedSwitch.SwitchManager;
                if (definedSwitch.SwitchManager != null)
                {
                    foreach (var carrier in TABS.CarrierAccount.All.Values)
                    {
                        foreach (string idIn in manager.GetCustomerCdrIdentifiers(definedSwitch, carrier))
                            TABS.DataHelper.ExecuteNonQuery(sql, definedSwitch.SwitchID, carrier.CarrierAccountID, idIn, 'Y', 'N');
                        foreach (string idOut in manager.GetSupplierCdrIdentifiers(definedSwitch, carrier))
                            TABS.DataHelper.ExecuteNonQuery(sql, definedSwitch.SwitchID, carrier.CarrierAccountID, idOut, 'N', 'Y');
                    }
                }
            }
        }

        public static object FindIdInTrafficStats(long id)
        {
            return TABS.DataHelper.ExecuteScalar(QueryBuilder.FindIdInTrafficStatsQuery(), id);
        }

        public static void DropAndCreateSwitchCarrierMapping()
        {
            TABS.DataHelper.ExecuteNonQuery(QueryBuilder.DropAndCreateSwitchCarrierMappingQuery());
        }

        public static DataTable GetSwitchCarrierMappings()
        {
            return TABS.DataHelper.GetDataTable(QueryBuilder.GetSwitchCarrierMappingsQuery());
        }

        public static IList<TABS.CurrencyExchangeRate> GetCurrencyExchange(TABS.Currency currency, DateTime rateUpdateDate)
        {
            NHibernate.IQuery query = QueryBuilder.GetCurrencyExchangeQuery(currency, rateUpdateDate);
            return query.List<TABS.CurrencyExchangeRate>();
        }

        public static IList<TABS.CurrencyExchangeRate> GetCurrencyExchangeHistory(string symbol)
        {
            TABS.Currency currency = TABS.Currency.All[symbol];
            return QueryBuilder.GetCurrencyExchangeHistoryQuery(currency)
                .List<TABS.CurrencyExchangeRate>();
        }

        public static IList<TABS.Commission> GetCommissions(TABS.CarrierAccount customer, TABS.CarrierAccount supplier, DateTime? effectiveDate)
        {
            return QueryBuilder.GetCommissionsQuery(customer, supplier, effectiveDate)
                .List<TABS.Commission>().Where(t => !t.Supplier.IsDeleted).ToList();
        }

        public static void DeleteSwitchReleaseCodes(int switchID)
        {
            TABS.DataHelper.ExecuteNonQuery(QueryBuilder.DeleteSwitchReleaseCodesQuery(), switchID);
        }

        public static IList<TABS.Tariff> GetTariffs(TABS.CarrierAccount customer, TABS.CarrierAccount supplier, DateTime? effectiveDate, TABS.Zone zone)
        {
            return QueryBuilder.GetTariffsQuery(customer, supplier, effectiveDate, zone)
                 .List<TABS.Tariff>().Where(t => !t.Supplier.IsDeleted).ToList();
        }

        public static IList<TABS.ToDConsideration> GetTODs(TABS.CarrierAccount customer, TABS.CarrierAccount supplier, DateTime? effectiveDate)
        {
            return QueryBuilder.GetTODsQuery(customer, supplier, effectiveDate)
                 .List<TABS.ToDConsideration>().Where(t => !t.Supplier.IsDeleted).ToList();
        }

        public static IList<TABS.Code> GetZoneCodes(int zoneid)
        {
            return QueryBuilder.GetZoneCodesQuery(zoneid)
                .List<TABS.Code>();
        }

        public static IList<TABS.Rate> GetRates(int? zoneid, TABS.CarrierAccount supplier, TABS.CarrierAccount customer)
        {
            return QueryBuilder.GetRatesQuery(zoneid, supplier, customer)
                .List<TABS.Rate>();
        }

        public static IList<TABS.Zone> GetOwnZonesForCustomer(TABS.CarrierAccount supplier, TABS.CarrierAccount customer)
        {
            return QueryBuilder.GetOwnZonesForCustomerQuery(supplier, customer).List<TABS.Zone>();
        }

        public static IList<TABS.Zone> GetAllSupplierZones(TABS.CarrierAccount supplier)
        {
            return QueryBuilder.GetAllSupplierZonesQuery(supplier).List<TABS.Zone>();
        }

        public static IList<PriceListChangeLog> GetPricelistChangeLog(PriceList pricelist)
        {
            return QueryBuilder.GetPricelistChangeLogQuery(pricelist)
                       .List<TABS.PriceListChangeLog>();
        }

        public static IList<Rate> GetRates(PriceList pricelist, DateTime effectivedate)
        {
           return QueryBuilder.GetRatesQuery(pricelist, effectivedate)
                    .List<TABS.Rate>();
        }

        public static IList<Code> GetSupplierCodes(CarrierAccount supplier, DateTime effectivedate)
        {
            return QueryBuilder.GetSupplierCodesQuery(supplier, effectivedate)
                     .List<TABS.Code>();
        }

        public static IList<PricingTemplatePlan> GetPricingTemplatePlan(int pricingtemplateid)
        {
            return QueryBuilder.GetPricingTemplatePlanQuery(pricingtemplateid)
                                                            .List<TABS.PricingTemplatePlan>();
        }

        public static int UpdateRoutingStatus(CarrierAccount carrier, string enabled)
        {
            string sql;
            if (enabled == "Enabled")
            {
                sql = "Update CarrierAccount Set RoutingStatus = 0 where CarrierAccountID = @P1";
            }
            else
                sql = "Update CarrierAccount Set RoutingStatus = 3 where CarrierAccountID = @P1";
            return DataHelper.ExecuteNonQuery(sql, carrier.CarrierAccountID);
        }

    }
}
