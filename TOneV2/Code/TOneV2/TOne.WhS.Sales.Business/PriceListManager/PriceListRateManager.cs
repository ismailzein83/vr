using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class PriceListRateManager
    {
        public void ProcessCountryRates(IProcessRatesContext context)
        {
            var newExplicitRates = new List<NewRate>();
            var customerPriceListsByCurrencyId = new Dictionary<int, List<NewPriceList>>();
            var productCustomersNewExplicitRates = new List<NewRate>();

            ProcessRates(new ProcessRatesInput()
            {
                ProcessInstanceId = context.ProcessInstanceId,
                UserId = context.UserId,
                OwnerType = context.OwnerType,
                OwnerId = context.OwnerId,
                PriceListCreationDate = context.PriceListCreationDate,
                CurrencyId = context.CurrencyId,
                LongPrecisionValue = context.LongPrecisionValue,
                RatesToChange = context.RatesToChange,
                RatesToClose = context.RatesToClose,
                ExistingRates = context.ExistingRates,
                ExistingZones = context.ExistingZones,
                ExplicitlyChangedExistingCustomerCountries = context.ExplicitlyChangedExistingCustomerCountries,
                InheritedRatesByZoneId = context.InheritedRatesByZoneId,
                NewExplicitRates = newExplicitRates,
                ProductCustomersNewExplicitRates = productCustomersNewExplicitRates,
                CustomerPriceListsByCurrencyId = customerPriceListsByCurrencyId
            });

            List<NewRate> newRates = new List<NewRate>();
            List<NewRate> newRatesToFillGapsDueToClosingCountry = new List<NewRate>();

            newRates.AddRange(context.RatesToChange.SelectMany(x => x.NewRates));
            newRatesToFillGapsDueToClosingCountry.AddRange(newExplicitRates);

            context.OwnerNewRates = newRates;
            context.NewRatesToFillGapsDueToClosingCountry = newRatesToFillGapsDueToClosingCountry;
            context.NewRatesToFillGapsDueToChangeSellingProductRates = productCustomersNewExplicitRates;
            context.ChangedRates = context.ExistingRates.Where(x => x.ChangedRate != null).Select(x => x.ChangedRate);
            context.CustomerPriceListsByCurrencyId = customerPriceListsByCurrencyId;
        }

        #region Private Methods

        private void ProcessRates(ProcessRatesInput input)
        {
            Dictionary<int, List<ExistingZone>> existingZonesByCountry;
            ExistingZonesByName existingZonesByName;
            Dictionary<long, ExistingZone> existingZonesByZoneId;
            StructureExistingZonesByCountryAndName(input.ExistingZones, out existingZonesByCountry, out existingZonesByName, out existingZonesByZoneId);

            ExistingRatesByZoneName existingRatesByZoneName = StructureExistingRatesByZoneName(input.ExistingRates);

            #region Process Rates To Change
            if (input.OwnerType == SalePriceListOwnerType.Customer)
                ProcessCustomerRatesToChange(input.OwnerId, input.RatesToChange, existingZonesByName, existingRatesByZoneName);
            else
            {
                ProcessProductRatesToChange(new ProcessProductRatesToChangeContext()
                {
                    ProcessInstanceId = input.ProcessInstanceId,
                    UserId = input.UserId,
                    SellingProductId = input.OwnerId,
                    PriceListCreationDate = input.PriceListCreationDate,
                    RatesToChange = input.RatesToChange,
                    CurrencyId = input.CurrencyId,
                    LongPrecisionValue = input.LongPrecisionValue,
                    ExistingZonesByZoneId = existingZonesByZoneId,
                    ExistingZonesByName = existingZonesByName,
                    ExistingRatesByZoneName = existingRatesByZoneName,
                    NewRates = input.ProductCustomersNewExplicitRates,//add to product customer ExplicitRates
                    CustomerPriceListsByCurrencyId = input.CustomerPriceListsByCurrencyId
                });
            }
            #endregion

            foreach (RateToClose rateToClose in input.RatesToClose)
            {
                IEnumerable<ExistingRate> matchExistingRates = GetMatchedExistingRates(existingRatesByZoneName, rateToClose.ZoneName, rateToClose.RateTypeId);
                if (matchExistingRates != null)
                    CloseExistingRates(rateToClose, matchExistingRates);
            }

            if (input.ExplicitlyChangedExistingCustomerCountries.Count() > 0)
            {
                Dictionary<int, CountryRange> endedCountryRangesByCountryId = GetEndedCountryRangesByCountryId(input.ExplicitlyChangedExistingCustomerCountries);

                var countryManager = new Vanrise.Common.Business.CountryManager();

                foreach (ExistingCustomerCountry changedExistingCountry in input.ExplicitlyChangedExistingCustomerCountries)
                {
                    int countryId = changedExistingCountry.CustomerCountryEntity.CountryId;
                    string countryName = countryManager.GetCountryName(countryId);

                    List<ExistingZone> matchedExistingZones = existingZonesByCountry.GetRecord(countryId);
                    if (matchedExistingZones == null || matchedExistingZones.Count == 0)
                        throw new DataIntegrityValidationException(string.Format("No existing zones for country '{0}' were found", countryName));

                    CountryRange countryRange = endedCountryRangesByCountryId.GetRecord(countryId);
                    if (countryRange == null)
                        throw new DataIntegrityValidationException(string.Format("The BED of country '{0}' was not found", countryName));
                    ProcessChangedExistingCountry(changedExistingCountry, matchedExistingZones, input.InheritedRatesByZoneId, countryRange, input.NewExplicitRates);
                }
            }
        }

        private void StructureExistingZonesByCountryAndName(IEnumerable<ExistingZone> existingZones, out Dictionary<int, List<ExistingZone>> existingZonesByCountry, out ExistingZonesByName existingZonesByName, out Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            existingZonesByCountry = new Dictionary<int, List<ExistingZone>>();
            existingZonesByName = new ExistingZonesByName();
            existingZonesByZoneId = new Dictionary<long, ExistingZone>();

            List<ExistingZone> zones;

            foreach (ExistingZone existingZone in existingZones)
            {
                if (!existingZonesByZoneId.ContainsKey(existingZone.ZoneId))
                    existingZonesByZoneId.Add(existingZone.ZoneId, existingZone);

                if (!existingZonesByCountry.TryGetValue(existingZone.CountryId, out zones))
                {
                    zones = new List<ExistingZone>();
                    existingZonesByCountry.Add(existingZone.CountryId, zones);
                }
                zones.Add(existingZone);

                if (!existingZonesByName.TryGetValue(existingZone.Name, out zones))
                {
                    zones = new List<ExistingZone>();
                    existingZonesByName.Add(existingZone.Name, zones);
                }
                zones.Add(existingZone);
            }
        }

        private ExistingRatesByZoneName StructureExistingRatesByZoneName(IEnumerable<ExistingRate> existingRates)
        {
            ExistingRatesByZoneName existingRatesByZoneName = new ExistingRatesByZoneName();
            List<ExistingRate> existingRatesList = null;

            if (existingRates != null)
            {
                foreach (ExistingRate item in existingRates)
                {
                    if (!existingRatesByZoneName.TryGetValue(item.ParentZone.Name, out existingRatesList))
                    {
                        existingRatesList = new List<ExistingRate>();
                        existingRatesByZoneName.Add(item.ParentZone.Name, existingRatesList);
                    }

                    existingRatesList.Add(item);
                }
            }

            return existingRatesByZoneName;
        }

        private IEnumerable<ExistingRate> GetMatchedExistingRates(ExistingRatesByZoneName ratesByZone, string zoneName, int? rateTypeId)
        {
            List<ExistingRate> matchedRates;
            if (ratesByZone.TryGetValue(zoneName, out matchedRates))
            {
                return matchedRates.FindAllRecords
                (
                    x => (!rateTypeId.HasValue && !x.RateEntity.RateTypeId.HasValue) ||
                    (rateTypeId.HasValue && x.RateEntity.RateTypeId.HasValue && x.RateEntity.RateTypeId.Value == rateTypeId.Value)
                );
            }
            return matchedRates;
        }

        #region Process Rates To Change

        #region Process Product Rates To Change
        private void ProcessProductRatesToChange(ProcessProductRatesToChangeContext context)
        {
            IEnumerable<int> customerIds = new CarrierAccountManager().GetCustomerIdsBySellingProductId(context.SellingProductId);
            IEnumerable<int> sellingProductIds = new List<int>() { context.SellingProductId };
            IEnumerable<long> zoneIds = context.RatesToChange.MapRecords(x => x.ZoneId);

            var saleZoneManager = new SaleZoneManager();
            var customerZoneRateHistoryLocator = new CustomerZoneRateHistoryLocator(new CustomerZoneRateHistoryReader(customerIds, sellingProductIds, zoneIds, true, false));

            int startingReservedPriceListId = (int)new SalePriceListManager().ReserveIdRange(customerIds.Count());

            Dictionary<int, int> reservedPricelistIdsPerCustomerId = new Dictionary<int, int>();
            foreach (int customerId in customerIds)
                reservedPricelistIdsPerCustomerId.Add(customerId, startingReservedPriceListId++);

            foreach (RateToChange rateToChange in context.RatesToChange)
            {
                ProcessOwnerRateToChange(rateToChange, context.ExistingZonesByName, context.ExistingRatesByZoneName);

                int? countryId = saleZoneManager.GetSaleZoneCountryId(rateToChange.ZoneId);
                if (!countryId.HasValue)
                    throw new NullReferenceException("countryId");

                ExistingZone existingZone = context.ExistingZonesByZoneId.GetRecord(rateToChange.ZoneId);

                ProcessProductRateToChange(new ProcessProductRateToChangeContext()
                {
                    ProcessInstanceId = context.ProcessInstanceId,
                    UserId = context.UserId,
                    SellingProductId = context.SellingProductId,
                    PriceListCreationDate = context.PriceListCreationDate,
                    CustomerIds = customerIds,
                    RateToChange = rateToChange,
                    ExistingZone = existingZone,
                    CountryId = countryId.Value,
                    CurrencyId = context.CurrencyId,
                    LongPrecisionValue = context.LongPrecisionValue,
                    CustomerZoneRateHistoryLocator = customerZoneRateHistoryLocator,
                    NewRates = context.NewRates,
                    CustomerPriceListsByCurrencyId = context.CustomerPriceListsByCurrencyId,
                    ReservedPricelistIdsPerCustomerId = reservedPricelistIdsPerCustomerId
                });
            }
        }
        private void ProcessProductRateToChange(ProcessProductRateToChangeContext context)
        {
            var productRateDateConfig = new ProductRateDateConfig() { BED = context.RateToChange.BED, EED = context.ExistingZone.EED };

            foreach (int customerId in context.CustomerIds)
            {
                IEnumerable<SaleRateHistoryRecord> customerZoneRateHistory =
                    context.CustomerZoneRateHistoryLocator.GetCustomerZoneRateHistory(customerId, context.SellingProductId, context.RateToChange.ZoneName, null, context.CountryId, context.CurrencyId, context.LongPrecisionValue);

                if (customerZoneRateHistory == null || customerZoneRateHistory.Count() == 0)
                    continue;

                IEnumerable<SaleRateHistoryRecord> overlappedCustomerZoneRateHistory =
                    Utilities.GetQIntersectT(new List<ProductRateDateConfig>() { productRateDateConfig }, customerZoneRateHistory.ToList(), SaleRateHistoryRecordMapper);

                if (overlappedCustomerZoneRateHistory == null || overlappedCustomerZoneRateHistory.Count() == 0)
                    continue;

                bool doSameRatesExist = false;
                int ratesCount = overlappedCustomerZoneRateHistory.Count();

                for (int i = 0; i < ratesCount; i++)
                {
                    SaleRateHistoryRecord nextRate = ((i + 1) < ratesCount) ? overlappedCustomerZoneRateHistory.ElementAt(i + 1) : null;
                    if (nextRate == null)
                        break;
                    SaleRateHistoryRecord currentRate = overlappedCustomerZoneRateHistory.ElementAt(i);
                    if (currentRate.SellingProductId.HasValue && !nextRate.SellingProductId.HasValue)
                    {
                        doSameRatesExist = true;
                        context.NewRates.Add(new NewRate()
                        {
                            PriceListId = context.ReservedPricelistIdsPerCustomerId.GetRecord(customerId),
                            RateId = currentRate.SaleRateId,
                            Zone = context.ExistingZone,
                            RateTypeId = null,
                            Rate = currentRate.Rate,
                            CurrencyId = null, // The rate's currency will be determined by the currency of its pricelist that'll be created below
                            BED = currentRate.BED,
                            EED = currentRate.EED,
                            ChangeType = RateChangeType.NotChanged
                        });
                    }
                }

                if (doSameRatesExist)
                {
                    List<NewPriceList> customerPriceLists = context.CustomerPriceListsByCurrencyId.GetOrCreateItem(context.CurrencyId, () => { return new List<NewPriceList>(); });
                    NewPriceList customerPriceList = customerPriceLists.FindRecord(x => x.OwnerId == customerId);
                    if (customerPriceList == null)
                    {
                        customerPriceLists.Add(new NewPriceList()
                        {
                            ProcessInstanceId = context.ProcessInstanceId,
                            UserId = context.UserId,
                            PriceListId = context.ReservedPricelistIdsPerCustomerId.GetRecord(customerId),
                            PriceListType = SalePriceListType.None,
                            OwnerType = SalePriceListOwnerType.Customer,
                            OwnerId = customerId,
                            CurrencyId = context.CurrencyId,
                            EffectiveOn = context.PriceListCreationDate
                        });
                    }
                }
            }
        }
        private class ProcessProductRatesToChangeContext
        {
            #region Input Properties
            public long ProcessInstanceId { get; set; }
            public int UserId { get; set; }
            public int SellingProductId { get; set; }
            public DateTime PriceListCreationDate { get; set; }
            public IEnumerable<RateToChange> RatesToChange { get; set; }
            public int CurrencyId { get; set; }
            public int LongPrecisionValue { get; set; }
            public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }
            public ExistingZonesByName ExistingZonesByName { get; set; }
            public ExistingRatesByZoneName ExistingRatesByZoneName { get; set; }
            #endregion

            #region Output Properties
            public List<NewRate> NewRates { get; set; }
            public Dictionary<int, List<NewPriceList>> CustomerPriceListsByCurrencyId { get; set; }
            #endregion
        }
        private class ProcessProductRateToChangeContext
        {
            #region Input Properties
            public int UserId { get; set; }
            public long ProcessInstanceId { get; set; }
            public int SellingProductId { get; set; }
            public DateTime PriceListCreationDate { get; set; }
            public IEnumerable<int> CustomerIds { get; set; }
            public RateToChange RateToChange { get; set; }
            public ExistingZone ExistingZone { get; set; }
            public int CountryId { get; set; }
            public int CurrencyId { get; set; }
            public int LongPrecisionValue { get; set; }
            public CustomerZoneRateHistoryLocator CustomerZoneRateHistoryLocator { get; set; }
            public Dictionary<int, int> ReservedPricelistIdsPerCustomerId { get; set; }
            #endregion

            #region Output Properties
            public List<NewRate> NewRates { get; set; }
            public Dictionary<int, List<NewPriceList>> CustomerPriceListsByCurrencyId { get; set; }
            #endregion
        }
        private class ProductRateDateConfig : IDateEffectiveSettingsEditable
        {
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
        }
        private Action<SaleRateHistoryRecord, SaleRateHistoryRecord> SaleRateHistoryRecordMapper = (saleRateHistoryRecord, targetSaleRateHistoryRecord) =>
        {
            targetSaleRateHistoryRecord.SaleRateId = saleRateHistoryRecord.SaleRateId;
            targetSaleRateHistoryRecord.Rate = saleRateHistoryRecord.Rate;
            targetSaleRateHistoryRecord.ConvertedRate = saleRateHistoryRecord.ConvertedRate;
            targetSaleRateHistoryRecord.PriceListId = saleRateHistoryRecord.PriceListId;
            targetSaleRateHistoryRecord.ChangeType = saleRateHistoryRecord.ChangeType;
            targetSaleRateHistoryRecord.CurrencyId = saleRateHistoryRecord.CurrencyId;
            targetSaleRateHistoryRecord.SellingProductId = saleRateHistoryRecord.SellingProductId;
            //targetSaleRateHistoryRecord.BED = saleRateHistoryRecord.BED;
            //targetSaleRateHistoryRecord.EED = saleRateHistoryRecord.EED;
            targetSaleRateHistoryRecord.SourceId = saleRateHistoryRecord.SourceId;
        };
        #endregion

        private void ProcessCustomerRatesToChange(int customerId, IEnumerable<RateToChange> ratesToChange, ExistingZonesByName existingZonesByName, ExistingRatesByZoneName existingRatesByZoneName)
        {
            foreach (RateToChange rateToChange in ratesToChange)
                ProcessOwnerRateToChange(rateToChange, existingZonesByName, existingRatesByZoneName);
        }

        #region Common Methods
        private void ProcessOwnerRateToChange(RateToChange rateToChange, ExistingZonesByName existingZonesByName, ExistingRatesByZoneName existingRatesByZoneName)
        {
            IEnumerable<ExistingRate> matchedExistingRates = GetMatchedExistingRates(existingRatesByZoneName, rateToChange.ZoneName, rateToChange.RateTypeId);
            if (matchedExistingRates != null)
            {
                bool shouldNotAddRate;
                ExistingRate recentExistingRate;
                CloseExistingOverlappedRates(rateToChange, matchedExistingRates, out shouldNotAddRate, out recentExistingRate);
                if (!shouldNotAddRate)
                {
                    rateToChange.ChangeType = RateChangeType.New;

                    if (recentExistingRate != null)
                    {
                        if (rateToChange.NormalRate > recentExistingRate.ConvertedRate)
                            rateToChange.ChangeType = RateChangeType.Increase;
                        else if (rateToChange.NormalRate < recentExistingRate.ConvertedRate)
                            rateToChange.ChangeType = RateChangeType.Decrease;
                        else rateToChange.ChangeType = RateChangeType.NotChanged;
                        rateToChange.RecentExistingRate = recentExistingRate;
                    }

                    ProcessRateToChange(rateToChange, existingZonesByName);
                }
            }
            else
            {
                rateToChange.ChangeType = RateChangeType.New;
                ProcessRateToChange(rateToChange, existingZonesByName);
            }
        }
        private void CloseExistingOverlappedRates(RateToChange rateToChange, IEnumerable<ExistingRate> matchExistingRates, out bool shouldNotAddRate, out ExistingRate recentExistingRate)
        {
            shouldNotAddRate = false;
            recentExistingRate = null;
            foreach (var existingRate in matchExistingRates)
            {
                if (existingRate.RateEntity.BED <= rateToChange.BED)
                    recentExistingRate = existingRate;
                if (existingRate.IsOverlappedWith(rateToChange))
                {
                    DateTime existingRateEED = Utilities.Max(rateToChange.BED, existingRate.BED);
                    existingRate.ChangedRate = new ChangedRate
                    {
                        RateId = existingRate.RateEntity.SaleRateId,
                        EED = existingRateEED
                    };
                    rateToChange.ChangedExistingRates.Add(existingRate);
                }
            }

        }
        private void ProcessRateToChange(RateToChange rateToChange, ExistingZonesByName existingZones)
        {
            List<ExistingZone> matchExistingZones;
            existingZones.TryGetValue(rateToChange.ZoneName, out matchExistingZones);


            DateTime currentRateBED = rateToChange.BED;
            bool shouldAddMoreRates = true;
            foreach (var zone in matchExistingZones.OrderBy(itm => itm.BED))
            {
                if (zone.EED.VRGreaterThan(zone.BED) && zone.EED.VRGreaterThan(currentRateBED) && rateToChange.EED.VRGreaterThan(zone.BED))
                {
                    AddNewRate(rateToChange, ref currentRateBED, zone, out shouldAddMoreRates);
                    if (!shouldAddMoreRates)
                        break;
                }
            }
        }
        private void AddNewRate(RateToChange rateToChange, ref DateTime currentRateBED, ExistingZone zone, out bool shouldAddMoreRates)
        {
            shouldAddMoreRates = false;
            var newRate = new NewRate
            {
                RateTypeId = rateToChange.RateTypeId,
                Rate = rateToChange.NormalRate,
                Zone = zone,
                BED = zone.BED > currentRateBED ? zone.BED : currentRateBED,
                EED = rateToChange.EED,
                ChangeType = rateToChange.ChangeType
            };
            if (newRate.EED.VRGreaterThan(zone.EED))//this means that zone has EED value
            {
                newRate.EED = zone.EED;
                currentRateBED = newRate.EED.Value;
                shouldAddMoreRates = true;
            }

            zone.NewRates.Add(newRate);

            rateToChange.NewRates.Add(newRate);
        }
        #endregion

        #endregion

        #region Process Rates To Close

        private void CloseExistingRates(RateToClose rateToClose, IEnumerable<ExistingRate> matchExistingRates)
        {
            foreach (var existingRate in matchExistingRates.OrderBy(x => x.BED))
            {
                if (existingRate.EED.VRGreaterThan(rateToClose.CloseEffectiveDate))
                {
                    existingRate.ChangedRate = new ChangedRate
                    {
                        RateId = existingRate.RateEntity.SaleRateId,
                        EED = Utilities.Max(rateToClose.CloseEffectiveDate, existingRate.BED)
                    };
                    rateToClose.ChangedExistingRates.Add(existingRate);
                }
            }
        }

        #endregion

        #region Process Changed Existing Countries

        private void ProcessChangedExistingCountry(ExistingCustomerCountry changedExistingCountry, IEnumerable<ExistingZone> matchedExistingZones, InheritedRatesByZoneId inheritedRatesByZoneId, CountryRange countryRange, List<NewRate> newExplicitRates)
        {
            foreach (ExistingZone existingZone in matchedExistingZones)
            {
                foreach (ExistingRate existingRate in existingZone.ExistingRates)
                {
                    if (existingRate.EED.VRGreaterThan(changedExistingCountry.ChangedCustomerCountry.EED))
                    {
                        existingRate.ChangedRate = new ChangedRate()
                        {
                            RateId = existingRate.RateEntity.SaleRateId,
                            EED = Vanrise.Common.Utilities.Max(existingRate.BED, changedExistingCountry.ChangedCustomerCountry.EED)
                        };
                    }
                }
                if (countryRange.EED.VRGreaterThan(countryRange.BED) && existingZone.BED < changedExistingCountry.EED.Value)
                    AddZoneExplicitRates(existingZone, inheritedRatesByZoneId.GetRecord(existingZone.ZoneId), countryRange, newExplicitRates);
            }
        }

        private Dictionary<int, CountryRange> GetEndedCountryRangesByCountryId(IEnumerable<ExistingCustomerCountry> changedExistingCountries)
        {
            var endedCountryRangesByCountryId = new Dictionary<int, CountryRange>();

            foreach (ExistingCustomerCountry endedCountry in changedExistingCountries)
            {
                int endedCountryId = endedCountry.CustomerCountryEntity.CountryId;

                if (!endedCountryRangesByCountryId.ContainsKey(endedCountryId))
                {
                    endedCountryRangesByCountryId.Add(endedCountryId, new CountryRange()
                    {
                        BED = Utilities.Max(endedCountry.BED, DateTime.Today),
                        EED = endedCountry.EED
                    });
                }
            }

            return endedCountryRangesByCountryId;
        }

        private void AddZoneExplicitRates(ExistingZone existingZone, ZoneInheritedRates zoneInheritedRates, CountryRange countryRange, List<NewRate> newExplicitRates)
        {
            if (zoneInheritedRates == null)
                throw new DataIntegrityValidationException(string.Format("No inherited rates were found for zone '{0}'", existingZone.Name));

            // Step 1: Prepare the inherited normal rates

            if (zoneInheritedRates.NormalRates == null || zoneInheritedRates.NormalRates.Count == 0)
                throw new DataIntegrityValidationException(string.Format("No inherited normal rates were found for zone '{0}'", existingZone.Name));

            Action<SaleRate, Rate> mapSaleRate = (saleRate, rate) =>
            {
                rate.RateTypeId = saleRate.RateTypeId;
                rate.RateValue = saleRate.Rate;
                rate.ZoneId = saleRate.ZoneId;
                rate.CurrencyId = saleRate.CurrencyId;
                rate.PriceListId = saleRate.PriceListId;
                rate.RateChange = saleRate.RateChange;
                rate.Source = SalePriceListOwnerType.SellingProduct;
            };
            var countryRangeAsList = new List<CountryRange>() { countryRange };
            IEnumerable<Rate> inheritedNormalRates = Utilities.GetQIntersectT<CountryRange, SaleRate, Rate>(countryRangeAsList, zoneInheritedRates.NormalRates, mapSaleRate);

            if (inheritedNormalRates == null || inheritedNormalRates.Count() == 0)
                throw new DataIntegrityValidationException(string.Format("No inherited normal rates were found for zone '{0}'", existingZone.Name));

            // Step 2: Prepare the existing explicit normal rates

            Func<ExistingRate, Rate> mapExistingRate = (existingRate) =>
            {
                return new Rate()
                {
                    RateTypeId = existingRate.RateEntity.RateTypeId,
                    RateValue = existingRate.RateEntity.Rate,
                    ZoneId = existingRate.RateEntity.ZoneId,
                    CurrencyId = existingRate.RateEntity.CurrencyId,
                    PriceListId = existingRate.RateEntity.PriceListId,
                    BED = existingRate.BED,
                    EED = existingRate.EED,
                    RateChange = existingRate.RateEntity.RateChange,
                    Source = SalePriceListOwnerType.Customer
                };
            };
            IEnumerable<Rate> explicitNormalRates = existingZone.ExistingRates.MapRecords(mapExistingRate, x => !x.RateEntity.RateTypeId.HasValue);

            // Step 3: Merge the inherited normal rates with the existing explicit normal ones

            Action<Rate, Rate> mapRate = (rate, rRate) =>
            {
                rRate.RateTypeId = rate.RateTypeId;
                rRate.RateValue = rate.RateValue;
                rRate.ZoneId = rate.ZoneId;
                rRate.CurrencyId = rate.CurrencyId;
                rRate.PriceListId = rate.PriceListId;
                rRate.RateChange = rate.RateChange;
                rRate.Source = rate.Source;
            };
            IEnumerable<Rate> mergedNormalRates = Utilities.MergeUnionWithQForce(inheritedNormalRates.ToList(), explicitNormalRates.ToList(), mapRate, mapRate);

            // Step 4: Create new explicit normal rates from the inherited normal ones that have resulted from the merge process

            foreach (Rate mergedNormalRate in mergedNormalRates)
            {
                if (mergedNormalRate.Source == SalePriceListOwnerType.SellingProduct)
                {
                    newExplicitRates.Add(new NewRate()
                    {
                        RateId = 0,
                        Zone = existingZone,
                        RateTypeId = mergedNormalRate.RateTypeId,
                        Rate = mergedNormalRate.RateValue,
                        CurrencyId = mergedNormalRate.CurrencyId,
                        BED = mergedNormalRate.BED,
                        EED = mergedNormalRate.EED,
                        ChangeType = mergedNormalRate.RateChange,
                    });
                }
            }
        }

        private class CountryRange : Vanrise.Entities.IDateEffectiveSettingsEditable
        {
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
        }

        private class Rate : Vanrise.Entities.IDateEffectiveSettings, Vanrise.Entities.IDateEffectiveSettingsEditable
        {
            public int? RateTypeId { get; set; }
            public decimal RateValue { get; set; }
            public long ZoneId { get; set; }
            public int? CurrencyId { get; set; }
            public int PriceListId { get; set; }
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
            public RateChangeType RateChange { get; set; }
            public SalePriceListOwnerType Source { get; set; }
        }

        #endregion

        #endregion

        #region Private Classes

        public class ProcessRatesInput
        {
            public long ProcessInstanceId { get; set; }
            public int UserId { get; set; }
            public SalePriceListOwnerType OwnerType { get; set; }
            public int OwnerId { get; set; }
            public DateTime PriceListCreationDate { get; set; }
            public int CurrencyId { get; set; }
            public int LongPrecisionValue { get; set; }
            public IEnumerable<RateToChange> RatesToChange { get; set; }
            public IEnumerable<RateToClose> RatesToClose { get; set; }
            public IEnumerable<ExistingRate> ExistingRates { get; set; }
            public IEnumerable<ExistingZone> ExistingZones { get; set; }
            public IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; set; }
            public InheritedRatesByZoneId InheritedRatesByZoneId { get; set; }
            public List<NewRate> NewExplicitRates { get; set; }
            public Dictionary<int, List<NewPriceList>> CustomerPriceListsByCurrencyId { get; set; }

            public List<NewRate> ProductCustomersNewExplicitRates { get; set; }
        }
        #endregion
    }
}
