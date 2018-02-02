using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business.Reader;
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
                allChanges.SubscriberOwnerEntities = newChanges.SubscriberOwnerEntities;
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

        public List<SubscriberOwnerEntity> GetDraftSubscriberOwnerEntities(SalePriceListOwnerType ownerType, int ownerId)
        {
            Changes draft = GetDraft(ownerType, ownerId);
            if (draft != null)
                return draft.SubscriberOwnerEntities;
            return null;
        }

        public SellingZonesWithDefaultRatesTaskData GetSellingZonesWithDefaultRatesTaskData(SalePriceListOwnerType ownerType, int ownerId)
        {
            IRatePlanDataManager ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return ratePlanDataManager.GetDraftTaskData(ownerType, ownerId, RatePlanStatus.Draft).SellingZonesWithDefaultRatesTaskData;
        }
        public void InsertOrUpdateDraftTaskData(SalePriceListOwnerType ownerType, int ownerId, DraftTaskData draftTaskData, RatePlanStatus status)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
             ratePlanDataManager.InsertOrUpdateDraftTaskData(ownerType, ownerId, draftTaskData, RatePlanStatus.Draft);
        }

        #region Define New Rates Converted To Currency

        public void DefineNewRatesConvertedToCurrency(DefineNewRatesConvertedToCurrencyInput input)
        {
            var ratePlanManager = new RatePlanManager();

            Changes draft = GetDraft(SalePriceListOwnerType.Customer, input.CustomerId);
            IEnumerable<SaleZone> allZones = ratePlanManager.GetSaleZones(SalePriceListOwnerType.Customer, input.CustomerId, input.EffectiveOn, true);

            var updatedZoneDrafts = new List<ZoneChanges>();
            var zoneDraftsByZoneId = new Dictionary<long, ZoneChanges>();

            if (draft != null)
            {
                draft.CurrencyId = input.NewCurrencyId;

                if (draft.ZoneChanges != null)
                    zoneDraftsByZoneId = draft.ZoneChanges.ToDictionary(x => x.ZoneId);
            }
            else
            {
                draft = new Changes();
                draft.CurrencyId = input.NewCurrencyId;
                draft.ZoneChanges = new List<ZoneChanges>();
            }

            if (allZones != null)
            {
                IEnumerable<long> filteredZoneIds;
                IEnumerable<SaleZone> filteredZones = GetFilteredZones(allZones, input.NewCountryIds, out filteredZoneIds);

                int sellingProductId = new CarrierAccountManager().GetSellingProductId(input.CustomerId);
                Dictionary<int, DateTime> countryBEDsByCountryId = UtilitiesManager.GetDatesByCountry(input.CustomerId, input.EffectiveOn, true);
                Dictionary<long, DateTime> zoneEffectiveDatesByZoneId = UtilitiesManager.GetZoneEffectiveDatesByZoneId(filteredZones, countryBEDsByCountryId);
                var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadRPChanges(input.CustomerId, sellingProductId, filteredZoneIds, DateTime.Today, zoneEffectiveDatesByZoneId));

                int longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();

                var saleRateManager = new SaleRateManager();
                var currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

                foreach (SaleZone zone in filteredZones)
                {
                    SaleEntityZoneRate zoneRate = rateLocator.GetCustomerZoneRate(input.CustomerId, sellingProductId, zone.SaleZoneId);

                    if (zoneRate != null && zoneRate.Rate != null && saleRateManager.GetCurrencyId(zoneRate.Rate) != input.NewCurrencyId)
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

                        DateTime newRateBED;

                        if (draft!=null && draft.CountryChanges!=null && draft.CountryChanges.NewCountries!=null && draft.CountryChanges.NewCountries.Any(item => item.CountryId == zone.CountryId))
                        {
                            if (!countryBEDsByCountryId.TryGetValue(zone.CountryId, out newRateBED))
                                throw new DataIntegrityValidationException(string.Format("The effective date of zone '{0}' was not found", zone.Name));
                            newRateBED = Utilities.Max(newRateBED, zone.BED);
                        }
                        else
                        {
                            if (!zoneEffectiveDatesByZoneId.TryGetValue(zone.SaleZoneId, out newRateBED))
                                throw new DataIntegrityValidationException(string.Format("The effective date of zone '{0}' was not found", zone.Name));
                        }

                        newRates.Add(new DraftRateToChange()
                        {
                            RateTypeId = null,
                            ZoneId = zone.SaleZoneId,
                            Rate = ConvertToCurrencyAndRound(zoneRate.Rate.Rate, saleRateManager.GetCurrencyId(zoneRate.Rate), input.NewCurrencyId, input.EffectiveOn, longPrecision, currencyExchangeRateManager),
                            CurrencyId = input.NewCurrencyId,
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
                                    Rate = ConvertToCurrencyAndRound(otherRate.Rate, saleRateManager.GetCurrencyId(otherRate), input.NewCurrencyId, input.EffectiveOn, longPrecision, currencyExchangeRateManager),
                                    CurrencyId = input.NewCurrencyId,
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
            SaveDraft(SalePriceListOwnerType.Customer, input.CustomerId, draft);
        }
        private IEnumerable<SaleZone> GetFilteredZones(IEnumerable<SaleZone> zones, IEnumerable<int> newCountryIds, out IEnumerable<long> filteredZoneIds)
        {
            var filteredZones = new List<SaleZone>();
            var filteredZoneIdsValue = new List<long>();

            bool doNewCountriesExist = (newCountryIds != null && newCountryIds.Count() > 0);

            foreach (SaleZone zone in zones)
            {
                if (!doNewCountriesExist || newCountryIds.Contains(zone.CountryId))
                {
                    filteredZones.Add(zone);
                    filteredZoneIdsValue.Add(zone.SaleZoneId);
                }
            }

            filteredZoneIds = filteredZoneIdsValue;
            return filteredZones;
        }

        #endregion

        #region Private Methods

        private decimal ConvertToCurrencyAndRound(decimal rate, int fromCurrencyId, int toCurrencyId, DateTime exchangeRateDate, int decimalPrecision, Vanrise.Common.Business.CurrencyExchangeRateManager exchangeRateManager)
        {
            return UtilitiesManager.ConvertToCurrencyAndRound(rate, fromCurrencyId, toCurrencyId, exchangeRateDate, decimalPrecision, exchangeRateManager);
        }

        #endregion
    }

    public class SaveCountryChangesInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public CountryChanges CountryChanges { get; set; }
    }
}
