using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanDraftManager
    {
        public Changes GetDraft(SalePriceListOwnerType ownerType, int ownerId)
        {
            IRatePlanDataManager ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return ratePlanDataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);
        }

        public bool DoesDraftExist(SalePriceListOwnerType ownerType, int ownerId)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            Changes draft = ratePlanDataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (draft == null)
                return false;

            if (draft.CountryChanges != null)
                return true;

            if (draft.ZoneChanges != null)
                return true;

            if (draft.DefaultChanges != null)
                if (draft.DefaultChanges.NewDefaultRoutingProduct != null || draft.DefaultChanges.DefaultRoutingProductChange != null)
                    return true;

            return false;
        }

        public CountryChanges GetCountryChanges(int customerId)
        {
            Changes draft = GetDraft(SalePriceListOwnerType.Customer, customerId);
            if (draft != null)
                return draft.CountryChanges;
            return null;
        }

        #region Save Draft

        public void SaveDraft(SalePriceListOwnerType ownerType, int ownerId, Changes newChanges)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();

            Changes existingChanges = ratePlanDataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);
            Changes allChanges = MergeChanges(existingChanges, newChanges);

            if (allChanges != null)
                ratePlanDataManager.InsertOrUpdateChanges(ownerType, ownerId, allChanges, RatePlanStatus.Draft);
        }

        private Changes MergeChanges(Changes existingChanges, Changes newChanges)
        {
            return Merge(existingChanges, newChanges, () =>
            {
                Changes allChanges = new Changes();

                allChanges.CurrencyId = newChanges.CurrencyId;
                allChanges.DefaultChanges = newChanges.DefaultChanges == null ? existingChanges.DefaultChanges : newChanges.DefaultChanges;

                IEnumerable<DraftNewCountry> existingCountries = (existingChanges.CountryChanges != null) ? existingChanges.CountryChanges.NewCountries : null;
                IEnumerable<DraftNewCountry> newCountries = (newChanges.CountryChanges != null) ? newChanges.CountryChanges.NewCountries : null;
                IEnumerable<int> removedCountryIds = GetRemovedCountryIds(existingCountries, newCountries);
                allChanges.CountryChanges = newChanges.CountryChanges;

                allChanges.ZoneChanges = MergeZoneChanges(existingChanges.ZoneChanges, newChanges.ZoneChanges, removedCountryIds);

                return allChanges;
            });
        }

        private IEnumerable<int> GetRemovedCountryIds(IEnumerable<DraftNewCountry> existingCountries, IEnumerable<DraftNewCountry> newCountries)
        {
            if (existingCountries == null)
                return null;

            if (newCountries == null)
                return existingCountries.MapRecords(x => x.CountryId);

            IEnumerable<int> existingCountryIds = existingCountries.MapRecords(x => x.CountryId);
            IEnumerable<int> newCountryIds = newCountries.MapRecords(x => x.CountryId);
            return existingCountryIds.FindAllRecords(x => !newCountryIds.Contains(x));
        }

        private List<ZoneChanges> MergeZoneChanges(List<ZoneChanges> existingZoneChanges, List<ZoneChanges> newZoneChanges, IEnumerable<int> removedCountryIds)
        {
            return Merge(existingZoneChanges, newZoneChanges, () =>
            {
                foreach (ZoneChanges existingZoneDraft in existingZoneChanges)
                {
                    if (!newZoneChanges.Any(x => x.ZoneId == existingZoneDraft.ZoneId))
                        newZoneChanges.Add(existingZoneDraft);
                }
                if (removedCountryIds != null)
                    return newZoneChanges.FindAll(x => !removedCountryIds.Contains(x.CountryId));
                else
                    return newZoneChanges;
            });
        }

        private T Merge<T>(T existingChanges, T newChanges, Func<T> mergeLogic) where T : class
        {
            if (existingChanges != null && newChanges != null)
                return mergeLogic();
            return existingChanges != null ? existingChanges : newChanges;
        }

        #endregion

        public bool DeleteDraft(SalePriceListOwnerType ownerType, int ownerId)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return ratePlanDataManager.CancelRatePlanChanges(ownerType, ownerId);
        }

        public int? GetDraftCurrencyId(SalePriceListOwnerType ownerType, int ownerId)
        {
            Changes draft = GetDraft(ownerType, ownerId);
            if (draft != null)
                return draft.CurrencyId;
            return null;
        }

        public void DefineNewRatesConvertedToCurrency(int customerId, int newCurrencyId, DateTime effectiveOn)
        {
            var ratePlanManager = new RatePlanManager();

            Changes draft = GetDraft(SalePriceListOwnerType.Customer, customerId);
            IEnumerable<SaleZone> allZones = ratePlanManager.GetSaleZones(SalePriceListOwnerType.Customer, customerId, effectiveOn, true);

            var updatedZoneDrafts = new List<ZoneChanges>();
            var zoneDraftsByZoneId = new Dictionary<long, ZoneChanges>();

            if (draft != null)
            {
                draft.CurrencyId = newCurrencyId;

                if (draft.ZoneChanges != null)
                    zoneDraftsByZoneId = draft.ZoneChanges.ToDictionary(x => x.ZoneId);
            }
            else
            {
                draft = new Changes();
                draft.CurrencyId = newCurrencyId;
                draft.ZoneChanges = new List<ZoneChanges>();
            }

            var saleRateManager = new SaleRateManager();
            var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));
            var currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            int? sellingProductId = new CarrierAccountManager().GetSellingProductId(customerId);
            if (!sellingProductId.HasValue)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a selling product", customerId));

            Dictionary<int, DateTime> countryBEDsByCountryId = UtilitiesManager.GetDatesByCountry(customerId, effectiveOn, true);
            int longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();

            if (allZones != null)
            {
                foreach (SaleZone zone in allZones)
                {
                    SaleEntityZoneRate zoneRate = rateLocator.GetCustomerZoneRate(customerId, sellingProductId.Value, zone.SaleZoneId);

                    if (zoneRate != null && zoneRate.Rate != null)
                    {
                        ZoneChanges zoneDraft = zoneDraftsByZoneId.GetOrCreateItem(zone.SaleZoneId, () =>
                        {
                            return new ZoneChanges()
                            {
                                ZoneId = zone.SaleZoneId,
                                ZoneName = zone.Name,
                                CountryId = zone.CountryId
                            };
                        });

                        var newRates = new List<DraftRateToChange>();
                        DateTime newRateBED = Utilities.Max(countryBEDsByCountryId.GetRecord(zone.CountryId), DateTime.Today);

                        newRates.Add(new DraftRateToChange()
                        {
                            RateTypeId = null,
                            ZoneId = zone.SaleZoneId,
                            Rate = ConvertToCurrencyAndRound(zoneRate.Rate.Rate, saleRateManager.GetCurrencyId(zoneRate.Rate), newCurrencyId, effectiveOn, longPrecision, currencyExchangeRateManager),
                            CurrencyId = newCurrencyId,
                            BED = newRateBED,
                            EED = null
                        });

                        if (zoneRate.RatesByRateType != null)
                        {
                            foreach (SaleRate otherRate in zoneRate.RatesByRateType.Values)
                            {
                                newRates.Add(new DraftRateToChange()
                                {
                                    RateTypeId = otherRate.RateTypeId,
                                    ZoneId = zone.SaleZoneId,
                                    Rate = ConvertToCurrencyAndRound(otherRate.Rate, saleRateManager.GetCurrencyId(otherRate), newCurrencyId, effectiveOn, longPrecision, currencyExchangeRateManager),
                                    CurrencyId = newCurrencyId,
                                    BED = newRateBED,
                                    EED = null
                                });
                            }
                        }

                        zoneDraft.NewRates = newRates;
                        updatedZoneDrafts.Add(zoneDraft);
                    }
                    else
                    {
                        ZoneChanges zoneDraft = zoneDraftsByZoneId.GetRecord(zone.SaleZoneId);

                        if (zoneDraft != null)
                        {
                            zoneDraft.NewRates = null;
                            updatedZoneDrafts.Add(zoneDraft);
                        }
                    }
                }
            }

            draft.ZoneChanges = updatedZoneDrafts;
            SaveDraft(SalePriceListOwnerType.Customer, customerId, draft);
        }

        private decimal ConvertToCurrencyAndRound(decimal rate, int fromCurrencyId, int toCurrencyId, DateTime exchangeRateDate, int decimalPrecision, Vanrise.Common.Business.CurrencyExchangeRateManager exchangeRateManager)
        {
            return UtilitiesManager.ConvertToCurrencyAndRound(rate, fromCurrencyId, toCurrencyId, exchangeRateDate, decimalPrecision, exchangeRateManager);
        }
    }

    public class SaveCountryChangesInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public CountryChanges CountryChanges { get; set; }
    }
}
