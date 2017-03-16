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
        public Vanrise.Entities.IDataRetrievalResult<SaleRateHistoryRecordDetail> GetFilteredSaleRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<SaleRateHistoryQuery> input)
        {
            return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData(input, new SaleRateHistoryRequestHandler());
        }

        private class SaleRateHistoryRequestHandler : Vanrise.Common.Business.BigDataRequestHandler<SaleRateHistoryQuery, SaleRateHistoryRecord, SaleRateHistoryRecordDetail>
        {
            #region Fields

            private SaleRateManager _saleRateManager = new SaleRateManager();
            private Vanrise.Common.Business.CurrencyManager _currencyManager = new Vanrise.Common.Business.CurrencyManager();
            private SellingProductManager _sellingProductManager = new SellingProductManager();

            #endregion

            public override IEnumerable<SaleRateHistoryRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleRateHistoryQuery> input)
            {
                IEnumerable<long> zoneIds = GetZoneIds(input.Query.OwnerType, input.Query.OwnerId, input.Query.SellingNumberPlanId, input.Query.CountryId, input.Query.ZoneName);

                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                    return GetSellingProductZoneRateHistoryRecords(input.Query.OwnerId, zoneIds);
                else
                    return GetCustomerZoneRateHistoryRecords(input.Query.OwnerId, zoneIds, input.Query.CountryId);
            }

            public override SaleRateHistoryRecordDetail EntityDetailMapper(SaleRateHistoryRecord entity)
            {
                return new SaleRateHistoryRecordDetail()
                {
                    Entity = entity,
                    CurrencySymbol = _currencyManager.GetCurrencySymbol(entity.CurrencyId),
                    SellingProductName = (entity.SellingProductId.HasValue) ? _sellingProductManager.GetSellingProductName(entity.SellingProductId.Value) : null
                };
            }

            #region Selling Product Methods

            private IEnumerable<SaleRateHistoryRecord> GetSellingProductZoneRateHistoryRecords(int sellingProductId, IEnumerable<long> zoneIds)
            {
                IEnumerable<SaleRate> spRates = _saleRateManager.GetAllSaleRatesByOwner(SalePriceListOwnerType.SellingProduct, sellingProductId, zoneIds);
                return (spRates != null && spRates.Count() > 0) ? PrepareSellingProductZoneRateHistoryRecords(spRates) : null;
            }

            private IEnumerable<SaleRateHistoryRecord> PrepareSellingProductZoneRateHistoryRecords(IEnumerable<SaleRate> spRates)
            {
                var records = new List<SaleRateHistoryRecord>();
                IEnumerable<SaleRate> orderedSPRates = spRates.OrderBy(x => x.BED);

                for (int i = 0; i < orderedSPRates.Count(); i++)
                {
                    SaleRate rate = orderedSPRates.ElementAt(i);

                    decimal? previousRateValue = null;
                    if (i > 0) previousRateValue = orderedSPRates.ElementAt(i - 1).Rate;

                    var record = new SaleRateHistoryRecord()
                    {
                        Rate = rate.Rate,
                        ChangeType = GetSaleRateChangeType(rate.Rate, previousRateValue),
                        CurrencyId = _saleRateManager.GetCurrencyId(rate),
                        BED = rate.BED,
                        EED = rate.EED
                    };

                    records.Add(record);
                }

                return records;
            }

            #endregion

            #region Customer Methods

            private IEnumerable<SaleRateHistoryRecord> GetCustomerZoneRateHistoryRecords(int customerId, IEnumerable<long> zoneIds, int countryId)
            {
                IEnumerable<SaleRateHistoryRecord> spIntersectedRates = GetSPIntersectedRates(customerId, zoneIds);
                if (spIntersectedRates == null || spIntersectedRates.Count() == 0)
                    return null;

                IEnumerable<SaleRateHistoryRecord> countryIntersectedRates = GetCountryIntersectedRates(customerId, countryId, spIntersectedRates);
                if (countryIntersectedRates == null || countryIntersectedRates.Count() == 0)
                    return null;

                IEnumerable<SaleRateHistoryRecord> customerIntersectedRates = GetCustomerIntersectedRates(customerId, zoneIds, countryIntersectedRates);
                if (customerIntersectedRates == null || customerIntersectedRates.Count() == 0)
                    return null;

                List<SaleRateHistoryRecord> customerIntersectedRatesAsList = customerIntersectedRates.ToList();
                PrepareCustomerZoneRateHistoryRecords(customerIntersectedRatesAsList);

                return customerIntersectedRatesAsList;
            }

            private Dictionary<int, List<ProcessedCustomerSellingProduct>> GetSPAssignmentsBySP(int customerId, out IEnumerable<int> spIds)
            {
                IEnumerable<ProcessedCustomerSellingProduct> spAssignments = new CustomerSellingProductManager().GetProcessedCustomerSellingProducts(customerId);

                if (spAssignments == null || spAssignments.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' has never been assigned to a SellingProduct", customerId));

                var spAssignmentsBySP = new Dictionary<int, List<ProcessedCustomerSellingProduct>>();
                var distinctSPIds = new List<int>();

                foreach (ProcessedCustomerSellingProduct spAssignment in spAssignments.OrderBy(x => x.BED))
                {
                    List<ProcessedCustomerSellingProduct> value;

                    if (!spAssignmentsBySP.TryGetValue(spAssignment.SellingProductId, out value))
                    {
                        value = new List<ProcessedCustomerSellingProduct>();
                        spAssignmentsBySP.Add(spAssignment.SellingProductId, value);
                    }

                    if (!distinctSPIds.Contains(spAssignment.SellingProductId))
                        distinctSPIds.Add(spAssignment.SellingProductId);

                    value.Add(spAssignment);
                }

                spIds = distinctSPIds;
                return spAssignmentsBySP;
            }

            private IEnumerable<SaleRateHistoryRecord> GetSPIntersectedRates(int customerId, IEnumerable<long> zoneIds)
            {
                IEnumerable<int> spIds;
                Dictionary<int, List<ProcessedCustomerSellingProduct>> spAssignmentsBySP = GetSPAssignmentsBySP(customerId, out spIds);
                Dictionary<int, List<SaleRate>> spRatesBySP = new SaleRateManager().GetZoneRatesBySellingProducts(zoneIds, spIds);

                var allIntersectedRecords = new List<SaleRateHistoryRecord>();

                foreach (int spId in spIds)
                {
                    List<ProcessedCustomerSellingProduct> spAssignments = spAssignmentsBySP.GetRecord(spId);
                    List<SaleRate> spRates = spRatesBySP.GetRecord(spId);

                    if (spRates == null || spRates.Count == 0)
                        continue;

                    Action<SaleRate, SaleRateHistoryRecord> SaleRateMapperAction = (saleRate, saleRateHistoryRecord) =>
                    {
                        saleRateHistoryRecord.Rate = saleRate.Rate;
                        saleRateHistoryRecord.CurrencyId = _saleRateManager.GetCurrencyId(saleRate);
                        saleRateHistoryRecord.SellingProductId = spId;
                    };

                    IEnumerable<SaleRateHistoryRecord> intersectedRecords = Vanrise.Common.Utilities.GetQIntersectT(spAssignments, spRates, SaleRateMapperAction);

                    if (intersectedRecords != null && intersectedRecords.Count() > 0)
                        allIntersectedRecords.AddRange(intersectedRecords);
                }

                return allIntersectedRecords.OrderBy(x => x.BED);
            }

            private IEnumerable<CustomerCountry2> GetCustomerCountries(int customerId, int countryId)
            {
                IEnumerable<CustomerCountry2> allCountries = new CustomerCountryManager().GetCustomerCountries(customerId);

                if (allCountries == null || allCountries.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("No Countries have ever been sold to Customer '{0}'", customerId));

                IEnumerable<CustomerCountry2> targetCountries = allCountries.FindAllRecords(x => x.CountryId == countryId);

                if (targetCountries == null || targetCountries.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Country '{0}' has never been sold to Customer '{0}'", countryId, customerId));

                return targetCountries.OrderBy(x => x.BED);
            }

            private IEnumerable<SaleRateHistoryRecord> GetCountryIntersectedRates(int customerId, int countryId, IEnumerable<SaleRateHistoryRecord> spIntersectedRates)
            {
                IEnumerable<CustomerCountry2> countries = GetCustomerCountries(customerId, countryId);

                List<CustomerCountry2> countriesAsList = countries.ToList();
                List<SaleRateHistoryRecord> spIntersectedRatesAsList = spIntersectedRates.ToList();

                return Vanrise.Common.Utilities.GetQIntersectT(countriesAsList, spIntersectedRatesAsList, MapSaleRateHistoryRecordAction);
            }

            private IEnumerable<SaleRateHistoryRecord> GetCustomerIntersectedRates(int customerId, IEnumerable<long> zoneIds, IEnumerable<SaleRateHistoryRecord> countryIntersectedRates)
            {
                IEnumerable<SaleRate> customerRates = new SaleRateManager().GetAllSaleRatesByOwner(SalePriceListOwnerType.Customer, customerId, zoneIds);

                if (customerRates == null || customerRates.Count() == 0)
                    return countryIntersectedRates;

                List<SaleRate> customerRatesAsList = customerRates.OrderBy(x => x.BED).ToList();
                List<SaleRateHistoryRecord> countryIntersectedRatesAsList = countryIntersectedRates.ToList();

                Action<SaleRate, SaleRateHistoryRecord> mapSaleRateAction = (rate, record) =>
                {
                    record.Rate = rate.Rate;
                    record.CurrencyId = _saleRateManager.GetCurrencyId(rate);
                    record.SellingProductId = null;
                };

                return Vanrise.Common.Utilities.MergeUnionWithQForce(countryIntersectedRatesAsList, customerRatesAsList, MapSaleRateHistoryRecordAction, mapSaleRateAction);
            }

            private void PrepareCustomerZoneRateHistoryRecords(List<SaleRateHistoryRecord> records)
            {
                for (int i = 0; i < records.Count; i++)
                {
                    SaleRateHistoryRecord record = records.ElementAt(i);

                    decimal? previousRateValue = null;
                    if (i > 0) previousRateValue = records.ElementAt(i - 1).Rate;

                    record.ChangeType = GetSaleRateChangeType(record.Rate, previousRateValue);
                }
            }

            #endregion

            #region Common Methods

            private IEnumerable<long> GetZoneIds(SalePriceListOwnerType ownerType, int ownerId, int? sellingNumberPlanId, int countryId, string zoneName)
            {
                int ownerSellingNumberPlanId = (sellingNumberPlanId.HasValue) ? sellingNumberPlanId.Value : GetOwnerSellingNumberPlanId(ownerType, ownerId);
                IEnumerable<long> zoneIds = new SaleZoneManager().GetSaleZoneIdsBySaleZoneName(ownerSellingNumberPlanId, countryId, zoneName);

                if (zoneIds == null || zoneIds.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SaleZoneIds of Zone '{0}' were not found", zoneName));

                return zoneIds;
            }

            private int GetOwnerSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
            {
                if (ownerType == SalePriceListOwnerType.SellingProduct)
                {
                    int? sellingNumberPlanId = new SellingProductManager().GetSellingNumberPlanId(ownerId);
                    if (!sellingNumberPlanId.HasValue)
                        throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SellingNumberPlanId of SellingProduct '{0}' was not found", ownerId));
                    return sellingNumberPlanId.Value;
                }
                else
                {
                    return new CarrierAccountManager().GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);
                }
            }

            private RateChangeType GetSaleRateChangeType(decimal rateValue, decimal? previousRateValue)
            {
                if (!previousRateValue.HasValue)
                    return RateChangeType.New;
                else if (rateValue > previousRateValue.Value)
                    return RateChangeType.Increase;
                else if (rateValue < previousRateValue.Value)
                    return RateChangeType.Decrease;
                else
                    return RateChangeType.NotChanged;
            }

            #endregion

            #region Mappers

            private Action<SaleRateHistoryRecord, SaleRateHistoryRecord> MapSaleRateHistoryRecordAction = (record, targetRecord) =>
            {
                targetRecord.Rate = record.Rate;
                targetRecord.ChangeType = record.ChangeType;
                targetRecord.CurrencyId = record.CurrencyId;
                targetRecord.SellingProductId = record.SellingProductId;
            };

            #endregion
        }
    }
}
