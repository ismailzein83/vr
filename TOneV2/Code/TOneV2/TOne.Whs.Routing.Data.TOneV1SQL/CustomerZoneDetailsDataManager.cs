using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;
using Vanrise.Rules;
using TOne.WhS.Routing.Business;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class CustomerZoneDetailsDataManager : RoutingDataManager, ICustomerZoneDetailsDataManager
    {
        public DateTime? EffectiveDate { get; set; }
        public bool? IsFuture { get; set; }

        Vanrise.Rules.RuleTree[] _ruleTreesForCustomerRoutes;

        Guid blockedRuleConfigId = new Guid("bbb0ca31-0fcd-4035-a8ed-5d4bad06c662");

        readonly string[] columns = { "CustomerId", "SaleZoneId", "RoutingProductId", "RoutingProductSource", "SellingProductId", "EffectiveRateValue", "RateSource", "SaleZoneServiceIds" };

        readonly string[] zoneRatesColumns = { "ZoneID", "SupplierID", "CustomerID", "ServicesFlag", "ProfileId", "ActiveRate", "IsTOD", "IsBlock", "CodeGroup" };

        Dictionary<int, CarrierAccount> _allCarriers;
        Dictionary<int, CarrierProfile> _allCarrierProfiles;
        Dictionary<long, SaleZone> _allSaleZones;
        Dictionary<int, ZoneServiceConfig> _allZoneServiceConfigs;

        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();
        CarrierProfileManager _carrierProfileManager = new CarrierProfileManager();
        SaleZoneManager _saleZoneManager = new SaleZoneManager();
        ZoneServiceConfigManager _zoneServiceConfigManager = new ZoneServiceConfigManager();
        public void ApplyCustomerZoneDetailsToDB(object preparedCustomerZoneDetails)
        {
            CustomerZoneDetailBulkInsertInfo customerZoneDetailBulkInsertInfo = preparedCustomerZoneDetails as CustomerZoneDetailBulkInsertInfo;
            Parallel.For(0, 2, (i) =>
            {
                switch (i)
                {
                    case 0: InsertBulkToTable(customerZoneDetailBulkInsertInfo.CustomerZoneDetailStreamForBulkInsertInfo); break;
                    case 1: InsertBulkToTable(customerZoneDetailBulkInsertInfo.ZoneRateStreamForBulkInsertInfo); break;
                }
            });
        }
        public IEnumerable<TOne.WhS.Routing.Entities.CustomerZoneDetail> GetCustomerZoneDetails()
        {
            return GetItemsText(query_GetCustomerZoneDetails, CustomerZoneDetailMapper, null);
        }
        public object FinishDBApplyStream(object dbApplyStream)
        {
            CustomerZoneDetailBulkInsert customerZoneDetailBulkInsert = dbApplyStream as CustomerZoneDetailBulkInsert;

            customerZoneDetailBulkInsert.CustomerZoneDetailStreamForBulkInsert.Close();
            customerZoneDetailBulkInsert.ZoneRateStreamForBulkInsert.Close();

            CustomerZoneDetailBulkInsertInfo customerZoneDetailBulkInsertInfo = new CustomerZoneDetailBulkInsertInfo();

            customerZoneDetailBulkInsertInfo.CustomerZoneDetailStreamForBulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CustomerZoneDetail]",
                Stream = customerZoneDetailBulkInsert.CustomerZoneDetailStreamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };

            customerZoneDetailBulkInsertInfo.ZoneRateStreamForBulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[ZoneRates_Temp]",
                Stream = customerZoneDetailBulkInsert.ZoneRateStreamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = zoneRatesColumns,
            };

            return customerZoneDetailBulkInsertInfo;
        }
        public object InitialiazeStreamForDBApply()
        {
            CustomerZoneDetailBulkInsert customerZoneDetailBulkInsert = new CustomerZoneDetailBulkInsert();
            customerZoneDetailBulkInsert.CustomerZoneDetailStreamForBulkInsert = base.InitializeStreamForBulkInsert();
            customerZoneDetailBulkInsert.ZoneRateStreamForBulkInsert = base.InitializeStreamForBulkInsert();
            return customerZoneDetailBulkInsert;
        }
        public void WriteRecordToStream(TOne.WhS.Routing.Entities.CustomerZoneDetail record, object dbApplyStream)
        {
            if (_allCarriers == null)
                _allCarriers = _carrierAccountManager.GetCachedCarrierAccounts();
            if (_allSaleZones == null)
                _allSaleZones = _saleZoneManager.GetCachedSaleZones();
            if (_allCarrierProfiles == null)
                _allCarrierProfiles = _carrierProfileManager.GetCachedCarrierProfiles();
            if (_allZoneServiceConfigs == null)
                _allZoneServiceConfigs = _zoneServiceConfigManager.GetCachedZoneServiceConfigs();

            int serviceflag = GetServiceFlag(record.SaleZoneServiceIds, _allZoneServiceConfigs);

            string saleZoneServiceIds = record.SaleZoneServiceIds != null ? string.Join(",", record.SaleZoneServiceIds) : null;

            CustomerZoneDetailBulkInsert customerZoneDetailBulkInsert = dbApplyStream as CustomerZoneDetailBulkInsert;
            customerZoneDetailBulkInsert.CustomerZoneDetailStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", record.CustomerId, record.SaleZoneId, record.RoutingProductId,
                (int)record.RoutingProductSource, record.SellingProductId, decimal.Round(record.EffectiveRateValue, 8), (int)record.RateSource, saleZoneServiceIds);

            if (!IsFuture.HasValue)
                throw new ArgumentNullException("IsFuture");

            if (!IsFuture.Value && !EffectiveDate.HasValue)
                throw new ArgumentNullException("EffectiveDate");


            var routeRuleTarget = new RouteRuleTarget
            {
                CustomerId = record.CustomerId,
                SaleZoneId = record.SaleZoneId,
                RoutingProductId = record.RoutingProductId,
                SaleRate = record.EffectiveRateValue,
                EffectiveOn = EffectiveDate,
                IsEffectiveInFuture = IsFuture.Value
            };
            RouteRule matchRule = GetRouteRule(routeRuleTarget);

            if (matchRule == null || matchRule.Settings.ConfigId != blockedRuleConfigId)
            {
                CarrierAccount customer = _allCarriers.GetRecord(record.CustomerId);
                CarrierProfile profile = _allCarrierProfiles.GetRecord(customer.CarrierProfileId);
                SaleZone saleZone = _allSaleZones.GetRecord(record.SaleZoneId);
                customerZoneDetailBulkInsert.ZoneRateStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}", saleZone.SourceId, "SYS", customer.SourceId, serviceflag, profile.SourceId, decimal.Round(record.EffectiveRateValue, 8), 0, 0, string.Empty);
            }
        }
        CustomerZoneDetail CustomerZoneDetailMapper(IDataReader reader)
        {
            string saleZoneServiceIds = reader["SaleZoneServiceIds"] as string;
            return new CustomerZoneDetail()
            {
                CustomerId = (int)reader["CustomerId"],
                EffectiveRateValue = GetReaderValue<decimal>(reader, "EffectiveRateValue"),
                RateSource = GetReaderValue<SalePriceListOwnerType>(reader, "RateSource"),
                RoutingProductId = GetReaderValue<int>(reader, "RoutingProductId"),
                RoutingProductSource = GetReaderValue<SaleEntityZoneRoutingProductSource>(reader, "RoutingProductSource"),
                SaleZoneId = (Int64)reader["SaleZoneId"],
                SellingProductId = GetReaderValue<int>(reader, "SellingProductId"),
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServiceIds) ? new HashSet<int>(saleZoneServiceIds.Split(',').Select(itm => int.Parse(itm))) : null
            };
        }

        public IEnumerable<CustomerZoneDetail> GetFilteredCustomerZoneDetailsByZone(IEnumerable<long> saleZoneIds)
        {
            DataTable dtZoneIds = BuildZoneIdsTable(new HashSet<long>(saleZoneIds));
            return GetItemsText(query_GetFilteredCustomerZoneDetailsByZone, CustomerZoneDetailMapper, (cmd) =>
            {

                var dtPrm = new SqlParameter("@ZoneList", SqlDbType.Structured);
                dtPrm.TypeName = "LongIDType";
                dtPrm.Value = dtZoneIds;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public List<CustomerZoneDetail> GetCustomerZoneDetails(HashSet<CustomerSaleZone> customerSaleZones)
        {
            throw new NotImplementedException();
        }

        DataTable BuildZoneIdsTable(HashSet<long> zoneIds)
        {
            DataTable dtZoneInfo = new DataTable();
            dtZoneInfo.Columns.Add("ZoneID", typeof(Int64));
            dtZoneInfo.BeginLoadData();
            foreach (var z in zoneIds)
            {
                DataRow dr = dtZoneInfo.NewRow();
                dr["ZoneID"] = z;
                dtZoneInfo.Rows.Add(dr);
            }
            dtZoneInfo.EndLoadData();
            return dtZoneInfo;
        }

        #region Queries

        const string query_GetCustomerZoneDetails = @"                                                       
                                            SELECT zd.[CustomerId]
                                                  ,zd.[SaleZoneId]
                                                  ,zd.[RoutingProductId]
                                                  ,zd.[RoutingProductSource]
                                                  ,zd.[SellingProductId]
                                                  ,zd.[EffectiveRateValue]
                                                  ,zd.[RateSource]
                                                  ,zd.[SaleZoneServiceIds]
                                              FROM [dbo].[CustomerZoneDetail] zd with(nolock)";

        const string query_GetFilteredCustomerZoneDetailsByZone = @"                                                       
                                            SELECT zd.[CustomerId]
                                                  ,zd.[SaleZoneId]
                                                  ,zd.[RoutingProductId]
                                                  ,zd.[RoutingProductSource]
                                                  ,zd.[SellingProductId]
                                                  ,zd.[EffectiveRateValue]
                                                  ,zd.[RateSource]
                                                  ,zd.[SaleZoneServiceIds]
                                              FROM [dbo].[CustomerZoneDetail] zd with(nolock)
                                              JOIN @ZoneList z ON z.ID = zd.SaleZoneID";

        #endregion
        private class CustomerZoneDetailBulkInsert
        {
            public StreamForBulkInsert CustomerZoneDetailStreamForBulkInsert { get; set; }
            public StreamForBulkInsert ZoneRateStreamForBulkInsert { get; set; }
        }

        private class CustomerZoneDetailBulkInsertInfo
        {
            public StreamBulkInsertInfo CustomerZoneDetailStreamForBulkInsertInfo { get; set; }
            public StreamBulkInsertInfo ZoneRateStreamForBulkInsertInfo { get; set; }
        }

        private RouteRule GetRouteRule(RouteRuleTarget routeRuleTarget)
        {
            if (_ruleTreesForCustomerRoutes == null)
                _ruleTreesForCustomerRoutes = new RouteRuleManager().GetRuleTreesByPriority(null);

            if (_ruleTreesForCustomerRoutes != null)
            {
                foreach (var ruleTree in _ruleTreesForCustomerRoutes)
                {
                    var matchRule = ruleTree.GetMatchRule(routeRuleTarget) as RouteRule;
                    if (matchRule != null)
                        return matchRule;
                }
            }
            return null;
        }


        public void UpdateCustomerZoneDetails(List<CustomerZoneDetail> customerZoneDetails)
        {
            throw new NotImplementedException();
        }


        public List<CustomerZoneDetail> GetCustomerZoneDetailsAfterVersionNumber(int versionNumber)
        {
            throw new NotImplementedException();
        }

        public List<CustomerZoneDetail> GetCustomerZoneDetailsByZoneIdsAndCustomerIds(List<long> saleZoneIds, List<int> customerIds)
        {
            throw new NotImplementedException();
        }
    }
}
