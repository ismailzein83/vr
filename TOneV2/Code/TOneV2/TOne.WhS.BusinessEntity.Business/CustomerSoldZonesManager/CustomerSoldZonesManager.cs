using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.Rules.Pricing;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerSoldZonesManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CustomersSoldZoneDetail> GetFilteredCustomerSoldZones(Vanrise.Entities.DataRetrievalInput<CustomerSoldZonesQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CustomerSoldZonesRequestHandler());
        }

        private class CustomerSoldZonesRequestHandler : BigDataRequestHandler<CustomerSoldZonesQuery, CustomersSoldZone, CustomersSoldZoneDetail>
        {
            #region Fields

            private CustomerCountryManager _customerCountryManager;
            private CarrierAccountManager _carrierAccountManager;
            private SaleZoneManager _saleZoneManager;
            private SaleCodeManager _saleCodeManager;
            private CurrencyExchangeRateManager _currencyExchangeRateManager;
            private Vanrise.Common.Business.ConfigManager _configManager;
            #endregion

            public CustomerSoldZonesRequestHandler()
            {
                _customerCountryManager = new CustomerCountryManager();
                _carrierAccountManager = new CarrierAccountManager();
                _saleZoneManager = new SaleZoneManager();
                _saleCodeManager = new SaleCodeManager();
                _currencyExchangeRateManager = new CurrencyExchangeRateManager();
                _configManager = new Vanrise.Common.Business.ConfigManager();

            }

            public override IEnumerable<CustomersSoldZone> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<CustomerSoldZonesQuery> input)
            {

                DateTime? dataBaseEffectiveTime = RoutingManagerFactory.GetManager<IRoutingDatabaseManager>().GetLatestRoutingDatabaseEffectiveTime(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
                if (!dataBaseEffectiveTime.HasValue)
                    return null;

                var customersIds = input.Query.CustomersIds == null ? _carrierAccountManager.GetCustomersIdsAssignedToSellingNumberPlanId(input.Query.SellingNumberPlanId) : input.Query.CustomersIds;

                DateTime effectiveOn = dataBaseEffectiveTime.Value;
                if (customersIds.Count == 0)
                    return null;

                var soldCustomersCountries = _customerCountryManager.GetCustomersCountriesEffectiveOrFuture(input.Query.CountriesIds, customersIds, effectiveOn);

                if (soldCustomersCountries.Count == 0)
                    return null;

                List<long> filterdZonesIds = new List<long>();
                if (input.Query.ZoneIds != null)
                    filterdZonesIds.AddRange(input.Query.ZoneIds);

                if (input.Query.Code != null)
                {
                    List<long> filterdZonesIdsByCode = _saleCodeManager.GetSaleZonesIdsByCode(input.Query.Code);

                    if (filterdZonesIdsByCode.Count == 0)
                        return null;

                    if (input.Query.ZoneIds != null)
                    {
                        foreach (var zoneId in input.Query.ZoneIds)
                        {
                            if (!filterdZonesIdsByCode.Contains(zoneId))
                                filterdZonesIds.Remove(zoneId);
                        }
                    }
                    else
                    {
                        filterdZonesIds.AddRange(filterdZonesIdsByCode);
                    }

                    if (filterdZonesIds.Count == 0)
                        return null;
                }

                var customerCountriesByCountryId = StructureCustomersCountriesByCountryId(soldCustomersCountries);

                var soldZonesIds = _saleZoneManager.GetSoldZonesBySellingNumberPlan(input.Query.SellingNumberPlanId, customerCountriesByCountryId.Keys.ToList(), filterdZonesIds, effectiveOn);

                if (soldZonesIds == null)
                    return null;
                var customerZoneDetails = RoutingManagerFactory.GetManager<ICustomerZoneDetailsManager>().GetCustomerZoneDetailsByZoneIdsAndCustomerIds(soldZonesIds, customersIds);

                var customerZoneDetailByZoneId = StructureCustomerZoneDetailByZoneId(customerZoneDetails);

                var customersSoldZone = new List<CustomersSoldZone>();

                int sysCurrencyId = _configManager.GetSystemCurrencyId();

                foreach (var customerZoneDetail in customerZoneDetailByZoneId)
                {
                    if (input.Query.RoutingProductsIds != null && input.Query.RoutingProductsIds.Count > 0 && !customerZoneDetail.Value.Any(x => input.Query.RoutingProductsIds.Contains(x.RoutingProductId)))
                        continue;

                    CustomersSoldZone customerSoldZone = new CustomersSoldZone();

                    customerSoldZone.ZoneId = customerZoneDetail.Key;

                    customerSoldZone.EffectiveOn = effectiveOn;

                    var customerZonesData = new List<CustomerZoneData>();


                    foreach (var customerRate in customerZoneDetail.Value)
                    {

                        if (input.Query.RoutingProductsIds != null && input.Query.RoutingProductsIds.Count > 0 && !input.Query.RoutingProductsIds.Contains(customerRate.RoutingProductId))
                            continue;

                        var context = new ExtraChargeRuleContext
                        {
                            TargetTime = DateTime.Now,
                            Rate = customerRate.EffectiveRateValue,
                            SourceCurrencyId = input.Query.CurrencyId
                        };

                        GetMatchRule(context, customerRate.CustomerId, customerSoldZone.ZoneId, DateTime.Now);
                        customerZonesData.Add(new CustomerZoneData
                        {
                            CustomerId = customerRate.CustomerId,
                            Rate = context.Rate,
                            RoutingProductId = customerRate.RoutingProductId,
                            Services = customerRate.SaleZoneServiceIds,
                            HasExtraCharge = context.ExtraChargeRate != 0 ? true : false
                        });
                    }

                    customerSoldZone.CustomerZoneData = customerZonesData.OrderBy(x => x.Rate).ThenBy(x => x.CustomerId).Take(input.Query.Top).ToList();
                    customerSoldZone.SaleCount = customerZonesData.Count;
                    customersSoldZone.Add(customerSoldZone);
                }

                return customersSoldZone;
            }
            private void GetMatchRule(IPricingRuleExtraChargeContext context, int customerId, long zoneId, DateTime effectiveDate)
            {
                var ruleDefinitionId = new Guid("90A47A0A-3EF9-4941-BC21-CA0BE44FC5A4");
                var target = new Vanrise.GenericData.Entities.GenericRuleTarget
                {
                    TargetFieldValues = new Dictionary<string, object>
                {
                    {"CustomerId", customerId},
                    {"SaleZoneId", zoneId}
                }
                };
                target.EffectiveOn = effectiveDate;
                new ExtraChargeRuleManager().ApplyExtraChargeRule(context, ruleDefinitionId, target);
            }


            public override CustomersSoldZoneDetail EntityDetailMapper(CustomersSoldZone entity)
            {
                var customerSoldZonesDetail = new CustomersSoldZoneDetail();
                customerSoldZonesDetail.ZoneId = entity.ZoneId;
                customerSoldZonesDetail.EffectiveOn = entity.EffectiveOn;
                customerSoldZonesDetail.Name = _saleZoneManager.GetSaleZoneName(entity.ZoneId);
                customerSoldZonesDetail.SaleCount = entity.SaleCount;
                customerSoldZonesDetail.CustomerZones = entity.CustomerZoneData.MapRecords(CustomerZoneDataDetailMapper);
                return customerSoldZonesDetail;
            }


            private CustomerZoneDataDetail CustomerZoneDataDetailMapper(CustomerZoneData customerZoneData)
            {
                return new CustomerZoneDataDetail()
                {
                    CustomerName = _carrierAccountManager.GetCarrierAccountName(customerZoneData.CustomerId),
                    Rate = customerZoneData.Rate,
                    RoutingProductId = customerZoneData.RoutingProductId,
                    Services = customerZoneData.Services,
                    HasExtraCharge = customerZoneData.HasExtraCharge
                };
            }

            protected override ResultProcessingHandler<CustomersSoldZoneDetail> GetResultProcessingHandler(DataRetrievalInput<CustomerSoldZonesQuery> input, BigResult<CustomersSoldZoneDetail> bigResult)
            {
                return new ResultProcessingHandler<CustomersSoldZoneDetail>
                {
                    ExportExcelHandler = new CustomerSoldZonesExcelExportHandler()
                };
            }

            #region Private / Protected Methods
            private Dictionary<int, List<CustomerCountry2>> StructureCustomersCountriesByCountryId(IEnumerable<CustomerCountry2> existingCustomersCountries)
            {
                var structuredCustomersCountries = new Dictionary<int, List<CustomerCountry2>>();

                foreach (CustomerCountry2 customerCountry2 in existingCustomersCountries)
                {
                    List<CustomerCountry2> customersCountries = structuredCustomersCountries.GetOrCreateItem(customerCountry2.CountryId);
                    customersCountries.Add(customerCountry2);
                }

                return structuredCustomersCountries;
            }



            private Dictionary<long, List<CustomerZoneDetail>> StructureCustomerZoneDetailByZoneId(IEnumerable<CustomerZoneDetail> customerZoneDetails)
            {
                var structuredCustomerZoneDetail = new Dictionary<long, List<CustomerZoneDetail>>();

                foreach (var customerZoneDetail in customerZoneDetails)
                {
                    List<CustomerZoneDetail> customerZoneDetailList = structuredCustomerZoneDetail.GetOrCreateItem(customerZoneDetail.SaleZoneId);
                    customerZoneDetailList.Add(customerZoneDetail);
                }

                return structuredCustomerZoneDetail;
            }

            #endregion
        }


        private class CustomerSoldZonesExcelExportHandler : ExcelExportHandler<CustomersSoldZoneDetail>
        {

            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CustomersSoldZoneDetail> context)
            {

                Vanrise.Entities.DataRetrievalInput<CustomerSoldZonesQuery> input = context.Input as Vanrise.Entities.DataRetrievalInput<CustomerSoldZonesQuery>;

                input.ThrowIfNull("input");

                ExportExcelSheet sheet = new ExportExcelSheet
                {
                    SheetName = "Least Sale Price",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone" });

                int topCustomers = input.Query.Top;
                for (var i = 0; i < topCustomers; i++)
                {
                    var j = i + 1;
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Customer " + j, Width = 30 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Sale Rate " + j, Width = 30 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services " + j, Width = 30 });
                }

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Name });
                            if (record.CustomerZones != null && record.CustomerZones.Any())
                            {
                                foreach (var customerZoneDataDetail in record.CustomerZones)
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = customerZoneDataDetail.CustomerName });
                                    row.Cells.Add(new ExportExcelCell { Value = customerZoneDataDetail.Rate });

                                    ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();
                                    var services = zoneServiceConfigManager.GetZoneServicesNames(customerZoneDataDetail.Services);
                                    string servicesSymbol = string.Join(",", services);
                                    row.Cells.Add(new ExportExcelCell { Value = servicesSymbol });
                                }
                                AddEmptyCelss(sheet.Header.Cells.Count, row.Cells.Count, row);
                            }
                        }
                    }
                }
                context.MainSheet = sheet;
            }
            private void AddEmptyCelss(int headerCellCount, int rowCellCount, ExportExcelRow row)
            {
                if (rowCellCount < headerCellCount)
                {
                    int emptyValuesCount = headerCellCount - rowCellCount;
                    for (int i = 0; i < emptyValuesCount; i++)
                    {
                        row.Cells.Add(new ExportExcelCell { Value = null });
                    }
                }
            }
        }
    }
}
