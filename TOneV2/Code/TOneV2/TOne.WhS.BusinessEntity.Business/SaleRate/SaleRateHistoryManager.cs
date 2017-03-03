using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateHistoryManager
    {
        #region Selling Product Members

        public Vanrise.Entities.IDataRetrievalResult<SellingProductZoneRateHistoryRecordDetail> GetFilteredSellingProductZoneRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<SellingProductZoneRateHistoryQuery> input)
        {
            return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData(input, new SellingProductZoneRateHistoryRequestHandler());
        }

        private class SellingProductZoneRateHistoryRequestHandler : Vanrise.Common.Business.BigDataRequestHandler<SellingProductZoneRateHistoryQuery, SellingProductZoneRateHistoryRecord, SellingProductZoneRateHistoryRecordDetail>
        {
            private Vanrise.Common.Business.CurrencyManager _currencyManager = new Vanrise.Common.Business.CurrencyManager();

            public override SellingProductZoneRateHistoryRecordDetail EntityDetailMapper(SellingProductZoneRateHistoryRecord entity)
            {
                return new SellingProductZoneRateHistoryRecordDetail()
                {
                    Entity = entity,
                    CurrencySymbol = _currencyManager.GetCurrencySymbol(entity.CurrencyId)
                };
            }

            public override IEnumerable<SellingProductZoneRateHistoryRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SellingProductZoneRateHistoryQuery> input)
            {
                int? sellingNumberPlanId = new SellingProductManager().GetSellingNumberPlanId(input.Query.SellingProductId);
                if (!sellingNumberPlanId.HasValue)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SellingNumberPlanId of SellingProduct '{0}' was not found", input.Query.SellingProductId));

                IEnumerable<long> zoneIds = new SaleZoneManager().GetSaleZoneIdsBySaleZoneName(sellingNumberPlanId.Value, input.Query.CountryId, input.Query.ZoneName);
                if (zoneIds == null || zoneIds.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SaleZoneIds of SaleZone '{0}' were not found", input.Query.ZoneName));

                var saleRateManager = new SaleRateManager();
                IEnumerable<SaleRate> saleRates = saleRateManager.GetAllSaleRatesByOwner(SalePriceListOwnerType.SellingProduct, input.Query.SellingProductId, zoneIds);

                if (saleRates == null)
                    return null;

                var records = new List<SellingProductZoneRateHistoryRecord>();
                IEnumerable<SaleRate> orderedRates = saleRates.OrderBy(x => x.BED);

                for (int i = 0; i < orderedRates.Count(); i++)
                {
                    SaleRate rate = saleRates.ElementAt(i);

                    var record = new SellingProductZoneRateHistoryRecord()
                    {
                        Rate = rate.Rate,
                        BED = rate.BED,
                        EED = rate.EED,
                        CurrencyId = saleRateManager.GetCurrencyId(rate),
                        ChangeType = RateChangeType.New
                    };

                    if (i == 0)
                        record.ChangeType = RateChangeType.New;
                    else
                    {
                        SaleRate previousRate = saleRates.ElementAt(i - 1);

                        if (rate.Rate > previousRate.Rate)
                            record.ChangeType = RateChangeType.Increase;
                        else if (rate.Rate < previousRate.Rate)
                            record.ChangeType = RateChangeType.Decrease;
                    }

                    records.Add(record);
                }

                return records;
            }
        }

        #endregion

        #region Customer Members

        public Vanrise.Entities.IDataRetrievalResult<CustomerZoneRateHistoryRecordDetail> GetFilteredCustomerZoneRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<CustomerZoneRateHistoryQuery> input)
        {
            return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData(input, new CustomerZoneRateHistoryRequestHandler());
        }

        private class CustomerZoneRateHistoryRequestHandler : Vanrise.Common.Business.BigDataRequestHandler<CustomerZoneRateHistoryQuery, CustomerZoneRateHistoryRecord, CustomerZoneRateHistoryRecordDetail>
        {
            private SellingProductManager _sellingProductManager = new SellingProductManager();
            private Vanrise.Common.Business.CurrencyManager _currencyManager = new Vanrise.Common.Business.CurrencyManager();
            private SaleRateManager _saleRateManager = new SaleRateManager();

            public override CustomerZoneRateHistoryRecordDetail EntityDetailMapper(CustomerZoneRateHistoryRecord entity)
            {
                return new CustomerZoneRateHistoryRecordDetail()
                {
                    Entity = entity,
                    SellingProductName = entity.SellingProductId.HasValue ? _sellingProductManager.GetSellingProductName(entity.SellingProductId.Value) : null,
                    CurrencySymbol = _currencyManager.GetCurrencySymbol(entity.CurrencyId)
                };
            }

            public override IEnumerable<CustomerZoneRateHistoryRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<CustomerZoneRateHistoryQuery> input)
            {
                int sellingNumberPlanId = new CarrierAccountManager().GetSellingNumberPlanId(input.Query.CustomerId, CarrierAccountType.Customer);
                IEnumerable<long> zoneIds = new SaleZoneManager().GetSaleZoneIdsBySaleZoneName(sellingNumberPlanId, input.Query.CountryId, input.Query.ZoneName);

                if (zoneIds == null || zoneIds.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SaleZoneIds of SaleZone '{0}' were not found", input.Query.ZoneName));

                IEnumerable<CustomerZoneRateHistoryRecord> recordsIntersectedWithProducts = GetRecordsIntersectedWithProducts(input.Query.CustomerId, zoneIds);
                if (recordsIntersectedWithProducts == null || recordsIntersectedWithProducts.Count() == 0)
                    return null;

                IEnumerable<CustomerZoneRateHistoryRecord> recordsIntersectedWithCountry = GetRecordsIntersectedWithCountry(input.Query.CustomerId, input.Query.CountryId, recordsIntersectedWithProducts);
                if (recordsIntersectedWithCountry == null || recordsIntersectedWithCountry.Count() == 0)
                    return null;

                IEnumerable<CustomerZoneRateHistoryRecord> recordsMergedWithCustomer = GetRecordsMergedWithCustomer(input.Query.CustomerId, zoneIds, recordsIntersectedWithCountry);

                if (recordsMergedWithCustomer == null || recordsMergedWithCustomer.Count() == 0)
                    return null;

                var records = new List<CustomerZoneRateHistoryRecord>();
                List<CustomerZoneRateHistoryRecord> orderedRecords = recordsMergedWithCustomer.OrderBy(x => x.BED).ToList();

                var saleRateManager = new SaleRateManager();

                for (int i = 0; i < orderedRecords.Count(); i++)
                {
                    CustomerZoneRateHistoryRecord record = orderedRecords.ElementAt(i);

                    if (i == 0)
                        record.ChangeType = RateChangeType.New;
                    else
                    {
                        CustomerZoneRateHistoryRecord previousRecord = orderedRecords.ElementAt(i - 1);

                        if (record.Rate > previousRecord.Rate)
                            record.ChangeType = RateChangeType.Increase;
                        else if (record.Rate < previousRecord.Rate)
                            record.ChangeType = RateChangeType.Decrease;
                    }
                }

                return orderedRecords;
            }

            #region Private Methods

            private IEnumerable<CustomerZoneRateHistoryRecord> GetRecordsIntersectedWithProducts(int customerId, IEnumerable<long> zoneIds)
            {
                IEnumerable<ProcessedCustomerSellingProduct> processedEntities = new CustomerSellingProductManager().GetProcessedCustomerSellingProducts(customerId);
                if (processedEntities == null || processedEntities.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' has not been assigned to any SellingProduct", customerId));

                IEnumerable<int> productIds = processedEntities.MapRecords(x => x.SellingProductId).Distinct();
                Dictionary<int, List<SaleRate>> ratesByProduct = new SaleRateManager().GetZoneRatesBySellingProducts(zoneIds, productIds);

                var allRecords = new List<CustomerZoneRateHistoryRecord>();

                foreach (ProcessedCustomerSellingProduct processedEntity in processedEntities)
                {
                    List<SaleRate> productRates = ratesByProduct.GetRecord(processedEntity.SellingProductId);

                    if (productRates == null || productRates.Count() == 0)
                        continue;

                    var processedEntityAsList = new List<ProcessedCustomerSellingProduct>() { processedEntity };
                    Action<SaleRate, CustomerZoneRateHistoryRecord> mapRateToRecord = (rate, record) =>
                    {
                        record.Rate = rate.Rate;
                        record.SellingProductId = processedEntity.SellingProductId;
                        record.CurrencyId = _saleRateManager.GetCurrencyId(rate);
                    };

                    IEnumerable<CustomerZoneRateHistoryRecord> records =
                        Vanrise.Common.Utilities.GetQIntersectT<ProcessedCustomerSellingProduct, SaleRate, CustomerZoneRateHistoryRecord>(processedEntityAsList, productRates, mapRateToRecord);

                    if (records != null && records.Count() > 0)
                        allRecords.AddRange(records);
                }

                return allRecords;
            }

            private IEnumerable<CustomerZoneRateHistoryRecord> GetRecordsIntersectedWithCountry(int customerId, int countryId, IEnumerable<CustomerZoneRateHistoryRecord> recordsIntersectedWithProducts)
            {
                IEnumerable<CustomerCountry2> countries = new CustomerCountryManager().GetCustomerCountries(customerId);
                IEnumerable<CustomerCountry2> filteredCountries = countries.FindAllRecords(x => x.CountryId == countryId);

                if (filteredCountries == null || filteredCountries.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Country '{0}' has never been sold to Customer '{1}'", countryId, customerId));

                IEnumerable<CustomerCountry2> orderedCountries = filteredCountries.OrderBy(x => x.BED);
                Action<CustomerZoneRateHistoryRecord, CustomerZoneRateHistoryRecord> mapRecordToRecord = (record, targetRecord) =>
                {
                    targetRecord.Rate = record.Rate;
                    targetRecord.SellingProductId = record.SellingProductId;
                    targetRecord.CurrencyId = record.CurrencyId;
                };

                return Vanrise.Common.Utilities.GetQIntersectT<CustomerCountry2, CustomerZoneRateHistoryRecord, CustomerZoneRateHistoryRecord>(orderedCountries.ToList(), recordsIntersectedWithProducts.ToList(), mapRecordToRecord);
            }

            private IEnumerable<CustomerZoneRateHistoryRecord> GetRecordsMergedWithCustomer(int customerId, IEnumerable<long> zoneIds, IEnumerable<CustomerZoneRateHistoryRecord> recordsIntersectedWithCountry)
            {
                IEnumerable<SaleRate> customerRates = new SaleRateManager().GetAllSaleRatesByOwner(SalePriceListOwnerType.Customer, customerId, zoneIds);
                if (customerRates == null || customerRates.Count() == 0)
                    return recordsIntersectedWithCountry;

                Dictionary<int, SalePriceList> priceListsById = new SalePriceListManager().GetCachedSalePriceLists();

                Action<CustomerZoneRateHistoryRecord, CustomerZoneRateHistoryRecord> mapRecordToRecord = (record, targetRecord) =>
                {
                    targetRecord.Rate = record.Rate;
                    targetRecord.SellingProductId = record.SellingProductId;
                    targetRecord.CurrencyId = record.CurrencyId;
                };

                Action<SaleRate, CustomerZoneRateHistoryRecord> mapRateToRecord = (rate, record) =>
                {
                    record.Rate = rate.Rate;
                    record.CurrencyId = _saleRateManager.GetCurrencyId(rate);
                };

                return Vanrise.Common.Utilities.MergeUnionWithQForce<CustomerZoneRateHistoryRecord, SaleRate, CustomerZoneRateHistoryRecord>(recordsIntersectedWithCountry.ToList(), customerRates.ToList(), mapRecordToRecord, mapRateToRecord);
            }

            #endregion
        }

        #endregion
    }
}
