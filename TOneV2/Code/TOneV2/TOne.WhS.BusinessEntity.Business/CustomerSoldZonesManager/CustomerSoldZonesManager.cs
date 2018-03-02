using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using Vanrise.Caching.Runtime;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

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
            private SaleRateManager _saleRateManager;

            #endregion

            public CustomerSoldZonesRequestHandler()
            {
                _customerCountryManager = new CustomerCountryManager();
                _carrierAccountManager = new CarrierAccountManager();
                _saleZoneManager = new SaleZoneManager();
                _saleCodeManager = new SaleCodeManager();
                _currencyExchangeRateManager = new CurrencyExchangeRateManager();
                _saleRateManager = new SaleRateManager();
            }

            public override IEnumerable<CustomersSoldZone> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<CustomerSoldZonesQuery> input)
            {
                var customersIds = input.Query.CustomersIds == null ? _carrierAccountManager.GetCustomersIdsAssignedToSellingNumberPlanId(input.Query.SellingNumberPlanId) : input.Query.CustomersIds;

                if (customersIds.Count == 0)
                    return null;

                var soldCustomersCountries = _customerCountryManager.GetCustomersCountriesEffectiveOrFuture(input.Query.CountriesIds, customersIds, input.Query.EffectiveOn);

                if (soldCustomersCountries.Count == 0)
                    return null;

                List<long> filterdZonesIdsByCode = null;

                if(input.Query.Code != null)
                {
                    filterdZonesIdsByCode = _saleCodeManager.GetSaleZonesIdsByCode(input.Query.Code);
                    
                    if (filterdZonesIdsByCode.Count == 0)
                        return null;
                }

                var customerCountriesByCountryId = StructureCustomersCountriesByCountryId(soldCustomersCountries);
                var soldZones = _saleZoneManager.GetSoldZonesBySellingNumberPlan(input.Query.SellingNumberPlanId, customerCountriesByCountryId.Keys.ToList(), filterdZonesIdsByCode, input.Query.ZoneName, input.Query.EffectiveOn);

                var distinctCustomerIds = soldCustomersCountries.Select(x => x.CustomerId).Distinct().ToList();

                var dataByCustomerList = _carrierAccountManager.GetRoutingCustomerInfoDetailsByCustomersIds(distinctCustomerIds);

                var sellingProductByCustomerId = StructureSellingProductByCustomerId(dataByCustomerList);

                var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadLastRateNoCache(dataByCustomerList, input.Query.EffectiveOn));
                var customerZoneRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadAllNoCache(distinctCustomerIds, input.Query.EffectiveOn, true));


                var customersSoldZone = new List<CustomersSoldZone>();
                foreach (SaleZone soldZone in soldZones)
                {
                    CustomersSoldZone customerSoldZone = new CustomersSoldZone();
                    customerSoldZone.ZoneId = soldZone.SaleZoneId;

                    var customerZonesData = new List<CustomerZoneData>();

                    var customerCountriesByZoneCountryId = customerCountriesByCountryId.GetRecord(soldZone.CountryId);

                    foreach (var customerCountry in customerCountriesByZoneCountryId)
                    {
                        int sellingProductId = sellingProductByCustomerId.GetRecord(customerCountry.CustomerId);
                        var saleRate = futureRateLocator.GetCustomerZoneRate(customerCountry.CustomerId, sellingProductId, soldZone.SaleZoneId);
                        if (saleRate == null)
                            throw new DataIntegrityValidationException(string.Format("Zone {0} does neither have an explicit rate nor a default rate set for selling product {1}.", soldZone.SaleZoneId, sellingProductId));

                        SaleEntityZoneRoutingProduct saleEntityZoneRoutingProduct = customerZoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerCountry.CustomerId, sellingProductId, soldZone.SaleZoneId);

                        if (saleEntityZoneRoutingProduct == null)
                            throw new DataIntegrityValidationException(string.Format("Customer {0} does not have an explicit routing product.", customerCountry.CustomerId));

                        if (input.Query.RoutingProductsIds != null && input.Query.RoutingProductsIds.Count > 0 && !input.Query.RoutingProductsIds.Contains(saleEntityZoneRoutingProduct.RoutingProductId))
                            continue;

                        int rateCurrency = _saleRateManager.GetCurrencyId(saleRate.Rate);

                        customerZonesData.Add(new CustomerZoneData
                        {
                            CustomerId = customerCountry.CustomerId,
                            Rate =_currencyExchangeRateManager.ConvertValueToCurrency(saleRate.Rate.Rate,rateCurrency, input.Query.CurrencyId, saleRate.Rate.BED),
                            RoutingProductId = saleEntityZoneRoutingProduct.RoutingProductId
                        });
                    }

                    customerSoldZone.CustomerZoneData = customerZonesData.OrderBy(x => x.Rate).ThenBy(x=>x.CustomerId).Take(input.Query.Top).ToList();
                    customerSoldZone.SaleCount = customerZonesData.Count;

                    customersSoldZone.Add(customerSoldZone);
                }

                return customersSoldZone;
            }


            public override CustomersSoldZoneDetail EntityDetailMapper(CustomersSoldZone entity)
            {
                var customerSoldZonesDetail = new CustomersSoldZoneDetail();
                customerSoldZonesDetail.ZoneId = entity.ZoneId;
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
                    RoutingProductId = customerZoneData.RoutingProductId
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

            private Dictionary<int, int> StructureSellingProductByCustomerId(IEnumerable<RoutingCustomerInfoDetails> routingCustomerInfoDetails)
            {
                var structuredRoutingCustomerInfoDetails = new Dictionary<int, int>();

                foreach (var routingCustomer in routingCustomerInfoDetails)
                {
                    int customerId;
                    if (!structuredRoutingCustomerInfoDetails.TryGetValue(routingCustomer.CustomerId, out customerId))
                    {
                        structuredRoutingCustomerInfoDetails.Add(routingCustomer.CustomerId, routingCustomer.SellingProductId);
                    }
                }

                return structuredRoutingCustomerInfoDetails;
            }


            #endregion
        }


        private class CustomerSoldZonesExcelExportHandler : ExcelExportHandler<CustomersSoldZoneDetail>
        {

            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CustomersSoldZoneDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Least Sale Price",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Sale Rates" , Width = 200});
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Count" });


                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.ZoneId});
                            row.Cells.Add(new ExportExcelCell { Value = record.Name });
                            row.Cells.Add(new ExportExcelCell { Value = GetCustomerZoneDataDetailsAsString(record.CustomerZones) });
                            row.Cells.Add(new ExportExcelCell { Value = record.SaleCount });
                        }
                    }
                }
                context.MainSheet = sheet;
            }

            private string GetCustomerZoneDataDetailsAsString(IEnumerable<CustomerZoneDataDetail> customerZoneDataDetails)
            {
                StringBuilder customerZoneDataSB = new StringBuilder();

                foreach (var customerZoneDataDetail in customerZoneDataDetails)
                {
                    customerZoneDataSB.Append(string.Format("{0} {1}", customerZoneDataDetail.CustomerName, customerZoneDataDetail.Rate)).Append("".PadRight(10));
                }

                return customerZoneDataSB.ToString();

            }
        }
    }
}
