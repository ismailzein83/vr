﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.CodePreparation.Business
{
    public class PriceListRateManager
    {
        public void ProcessCountryRates(IProcessCountryRatesContext context, SalePriceListsByOwner salePriceListsByOwner)
        {
            ProcessCountryRates(context.ZonesToProcess, context.ExistingZones, context.ExistingRates, salePriceListsByOwner, context.NotImportedZones, context.EffectiveDate, context.SellingNumberPlanId);
            context.NewRates = context.ZonesToProcess.SelectMany(item => item.RatesToAdd).SelectMany(itm => itm.AddedRates);
            context.ChangedRates = context.ExistingRates.Where(itm => itm.ChangedRate != null).Select(itm => itm.ChangedRate);
            context.NewZonesRoutingProducts = context.ZonesToProcess.SelectMany(item => item.ZonesRoutingProductsToAdd).SelectMany(item => item.AddedZonesRoutingProducts);
        }

        private void ProcessCountryRates(IEnumerable<ZoneToProcess> zonesToProcess, IEnumerable<ExistingZone> existingZones, IEnumerable<ExistingRate> existingRates,
            SalePriceListsByOwner salePriceListsByOwner, IEnumerable<NotImportedZone> notImportedZones, DateTime effectiveDate, int sellingNumberPlanId)
        {
            ExistingRateGroupByZoneName existingRateGroupByZoneName = StructureExistingRatesByZoneName(existingRates);
            ProcessNotImportedData(notImportedZones, existingZones, existingRateGroupByZoneName);
            ProcessImportedData(zonesToProcess, existingZones, existingRates, existingRateGroupByZoneName, salePriceListsByOwner, effectiveDate, sellingNumberPlanId);

        }

        private void ProcessImportedData(IEnumerable<ZoneToProcess> zonesToProcess, IEnumerable<ExistingZone> existingZones, IEnumerable<ExistingRate> existingRates,
            ExistingRateGroupByZoneName existingRateGroupByZoneName, SalePriceListsByOwner salePriceListsByOwner, DateTime effectiveDate, int sellingNumberPlanId)
        {
            //If no existing zones exist, no need to perform the whole process
            if (!existingZones.Any())
                return;

            ExistingRateGroup existingRateGroup;
            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                existingRateGroupByZoneName.TryGetValue(zoneToProcess.ZoneName, out existingRateGroup);

                if (zoneToProcess.ChangeType == ZoneChangeType.New || zoneToProcess.ChangeType == ZoneChangeType.Renamed)
                    ManageRate(zoneToProcess, existingRates, existingZones, salePriceListsByOwner, effectiveDate);
                else
                    PrepareDataForPreview(zoneToProcess, existingRateGroup);
            }
        }

        #region Manage rates

        private void ManageRate(ZoneToProcess zoneToProcess, IEnumerable<ExistingRate> existingRates, IEnumerable<ExistingZone> existingZones, SalePriceListsByOwner salePriceListsByOwner, DateTime effectiveDate)
        {
            IEnumerable<ExistingRate> effectiveExistingRates = existingRates.FindAllRecords(itm => itm.EED == itm.ParentZone.EED);
            ExistingRatesByZoneName effectiveExistingRatesByZoneName = StructureEffectiveExistingRatesByZoneName(effectiveExistingRates);

            SettingManager settingManager = new SettingManager();
            SaleAreaSettingsData saleAreaSettingsData = settingManager.GetSetting<SaleAreaSettingsData>(BusinessEntity.Business.Constants.SaleAreaSettings);
            Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType = StructureZonesByType(existingZones, saleAreaSettingsData);

            IEnumerable<ExistingZone> fixedZones = zonesByType[SaleZoneTypeEnum.Fixed];
            IEnumerable<ExistingZone> mobileZones = zonesByType[SaleZoneTypeEnum.Mobile];
            SaleZoneTypeEnum saleZoneType = GetSaleZoneType(zoneToProcess.ZoneName, saleAreaSettingsData);

            if (!fixedZones.Any() && !mobileZones.Any())
                return;

            IEnumerable<NewZoneRateEntity> rates;

            if (saleZoneType == SaleZoneTypeEnum.Mobile && !mobileZones.Any() && fixedZones.Any())
            {
                rates = CreateRatesWithDefaultValue(fixedZones.Select(z => z.Name), effectiveExistingRatesByZoneName);
            }
            else if (saleZoneType == SaleZoneTypeEnum.Fixed && !fixedZones.Any() && mobileZones.Any())
            {
                rates = CreateRatesWithDefaultValue(mobileZones.Select(z => z.Name), effectiveExistingRatesByZoneName);
            }
            else
            {
                IEnumerable<string> matchedZoneNames = GetMatchedZones(zoneToProcess, saleZoneType, zonesByType);
                rates = GetHighestRatesFromZoneMatchesSaleEntities(matchedZoneNames, effectiveExistingRatesByZoneName);
            }
            AddRateToAddToZoneToProcess(zoneToProcess, effectiveDate, salePriceListsByOwner, rates);
        }

        private List<string> GetMatchedZones(ZoneToProcess zoneToProcess, SaleZoneTypeEnum saleZoneType, Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType)
        {
            List<string> matchedZoneNames = new List<string>();
            string recentZoneName = zoneToProcess.ChangeType == ZoneChangeType.Renamed
                ? zoneToProcess.RecentZoneName
                : zoneToProcess.SplitByZoneName;

            if (!string.IsNullOrEmpty(recentZoneName))
                matchedZoneNames.Add(recentZoneName);

            else
            {
                List<ExistingZone> matchingZones = new List<ExistingZone>();
                List<string> codes = GetCodes(zoneToProcess.CodesToMove, zoneToProcess.CodesToAdd);

                if (!codes.Any()) throw new Exception(string.Format("A new zone '{0}' does not have any new codes", zoneToProcess.ZoneName));

                IEnumerable<ExistingZone> fixedZones = zonesByType[SaleZoneTypeEnum.Fixed];
                IEnumerable<ExistingZone> mobileZones = zonesByType[SaleZoneTypeEnum.Mobile];

                if (fixedZones.Any() && saleZoneType == SaleZoneTypeEnum.Fixed)
                    matchingZones = GetMatchedExistingZones(codes, fixedZones);
                if (mobileZones.Any() && saleZoneType == SaleZoneTypeEnum.Mobile)
                    matchingZones = GetMatchedExistingZones(codes, mobileZones);

                matchedZoneNames = matchingZones.Select(z => z.Name).ToList();
            }
            return matchedZoneNames;
        }

        private List<string> GetCodes(IEnumerable<CodeToMove> codesToMove, IEnumerable<CodeToAdd> codesToAdd)
        {
            List<string> codes = new List<string>();
            if (codesToMove.Any())
                codes.AddRange(codesToMove.Select(c => c.Code));
            if (codesToAdd.Any())
                codes.AddRange(codesToAdd.Select(c => c.Code));

            return codes;
        }
        private void AddRateToAddToZoneToProcess(ZoneToProcess zoneToProcess, DateTime effectiveDate, SalePriceListsByOwner salePriceListsByOwner, IEnumerable<NewZoneRateEntity> rates)
        {
            SaleEntityZoneRoutingProductLocator effectiveRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(effectiveDate));

            CustomerCountryManager customerCountryManager = new CustomerCountryManager();

            Dictionary<int, Dictionary<int, DateTime>> countriesByCustomerId = new Dictionary<int, Dictionary<int, DateTime>>();
            Dictionary<int, DateTime> customerCountriesByCountryId;

            List<CustomerCountry2> customerCountries;


            foreach (var rate in rates)
            {
                if (rate.OwnerType == SalePriceListOwnerType.Customer)
                {
                    customerCountriesByCountryId = countriesByCustomerId.GetRecord(rate.OwnerId);
                    if (customerCountriesByCountryId == null)
                    {
                        customerCountriesByCountryId = new Dictionary<int, DateTime>();
                        customerCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(rate.OwnerId, effectiveDate).ToList();
                        foreach (var customerCountry in customerCountries)
                        {
                            DateTime countrySellDate;
                            if (!customerCountriesByCountryId.TryGetValue(customerCountry.CountryId, out countrySellDate) || customerCountry.BED < countrySellDate)
                                customerCountriesByCountryId.Add(customerCountry.CountryId, customerCountry.BED);
                        }
                        countriesByCustomerId.Add(rate.OwnerId, customerCountriesByCountryId);
                    }
                }

                PriceListToAdd priceListToAdd = new PriceListToAdd
                {
                    OwnerId = rate.OwnerId,
                    OwnerType = rate.OwnerType,
                    EffectiveOn = effectiveDate,
                    CurrencyId = rate.CurrencyId ?? GetOwnerCurreny(rate.OwnerId, rate.OwnerType)
                };

                priceListToAdd = salePriceListsByOwner.TryAddValue(priceListToAdd);

                RateToAdd rateToAdd = new RateToAdd
                {
                    PriceListToAdd = priceListToAdd,
                    Rate = rate.Rate,
                    ZoneName = zoneToProcess.ZoneName,
                    CurrencyId = rate.CurrencyId
                };

                foreach (AddedZone addedZone in zoneToProcess.AddedZones)
                {
                    DateTime countrySellDate = DateTime.MinValue;
                    if (rate.OwnerType == SalePriceListOwnerType.Customer)
                    {
                        if (!countriesByCustomerId.TryGetValue(rate.OwnerId, out customerCountriesByCountryId))
                            throw new Exception(string.Format("Customer with Id {0} do not have any sold country", rate.OwnerId));

                        if (!customerCountriesByCountryId.TryGetValue(addedZone.CountryId, out countrySellDate))
                            throw new Exception(string.Format("Country with Id {0} is not sold to Customer with Id {1}", addedZone.CountryId, rate.OwnerId));
                    }

                    rateToAdd.AddedRates.Add(new AddedRate
                    {
                        BED = addedZone.BED > countrySellDate ? addedZone.BED : countrySellDate,
                        EED = addedZone.EED,
                        PriceListToAdd = rateToAdd.PriceListToAdd,
                        NormalRate = rateToAdd.Rate,
                        AddedZone = addedZone
                    });
                }
                zoneToProcess.RatesToAdd.Add(rateToAdd);

                var saleEntityZoneRoutingProduct = this.GetSaleEntityZoneRoutingProduct(rate.OwnerType, rate.OwnerId, rate.HighesRateZoneId, effectiveRoutingProductLocator);
                ZoneRoutingProductToAdd zoneRoutingProductToAdd = new ZoneRoutingProductToAdd
                {
                    ZoneName = zoneToProcess.ZoneName,
                    OwnerId = rate.OwnerId,
                    OwnerType = rate.OwnerType,
                    RoutingProductId = saleEntityZoneRoutingProduct.RoutingProductId
                };

                foreach (AddedZone addedZone in zoneToProcess.AddedZones)
                {
                    zoneRoutingProductToAdd.AddedZonesRoutingProducts.Add(new AddedZoneRoutingProduct
                    {
                        RoutingProductId = saleEntityZoneRoutingProduct.RoutingProductId,
                        OwnerType = rate.OwnerType,
                        OwnerId = rate.OwnerId,
                        BED = addedZone.BED > saleEntityZoneRoutingProduct.BED ? addedZone.BED : saleEntityZoneRoutingProduct.BED,
                        //EED = addedZone.EED < saleEntityZoneRoutingProduct.EED ? addedZone.EED : saleEntityZoneRoutingProduct.EED,
                        EED = null,
                        AddedZone = addedZone,
                    });
                }
                zoneToProcess.ZonesRoutingProductsToAdd.Add(zoneRoutingProductToAdd);

            }
        }

        private SaleEntityZoneRoutingProduct GetSaleEntityZoneRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, long? highesRateZoneId, SaleEntityZoneRoutingProductLocator effectiveRoutingProductLocator)
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SaleEntityZoneRoutingProduct saleEntityZoneRoutingProduct = null;

            int sellingProductId = 0;

            if (ownerType == SalePriceListOwnerType.Customer)
                sellingProductId = carrierAccountManager.GetSellingProductId(ownerId);

            if (highesRateZoneId.HasValue)
            {
                if (ownerType == SalePriceListOwnerType.SellingProduct)
                    saleEntityZoneRoutingProduct = effectiveRoutingProductLocator.GetSellingProductZoneRoutingProduct(ownerId, highesRateZoneId.Value);
                else
                    saleEntityZoneRoutingProduct = effectiveRoutingProductLocator.GetCustomerZoneRoutingProduct(ownerId, sellingProductId, highesRateZoneId.Value);
            }

            if (saleEntityZoneRoutingProduct != null)
            {
                var routingProduct = routingProductManager.GetRoutingProduct(saleEntityZoneRoutingProduct.RoutingProductId);
                if (routingProduct.Settings.ZoneRelationType == RoutingProductZoneRelationType.AllZones)
                    return saleEntityZoneRoutingProduct;
            }

            if (ownerType == SalePriceListOwnerType.SellingProduct)
                return effectiveRoutingProductLocator.GetSellingProductDefaultRoutingProduct(ownerId);
            return effectiveRoutingProductLocator.GetCustomerDefaultRoutingProduct(ownerId, sellingProductId);
        }

        private List<ExistingZone> GetMatchedExistingZones(IEnumerable<string> codes, IEnumerable<ExistingZone> existingZonesByType)
        {
            List<ExistingZone> matchedExistingZones = new List<ExistingZone>();

            List<SaleCode> saleCodes = new List<SaleCode>();

            foreach (ExistingZone existingZone in existingZonesByType)
            {
                saleCodes.AddRange(existingZone.ExistingCodes.Select(item => item.CodeEntity));
            }

            CodeIterator<SaleCode> codeIterator = new CodeIterator<SaleCode>(saleCodes);
            foreach (string code in codes)
            {
                SaleCode matchedCode = codeIterator.GetLongestMatch(code);
                if (matchedCode != null)
                {
                    ExistingZone existingZone = existingZonesByType.FindRecord(item => item.ZoneId == matchedCode.ZoneId);
                    if (!matchedExistingZones.Contains(existingZone))
                        matchedExistingZones.Add(existingZone);
                }
            }
            //In case no matching zones from new codes to add, take all zones from matching type, i.e: all mobile zones or all fixed zones
            return matchedExistingZones.Any() ? matchedExistingZones : existingZonesByType.ToList();
        }

        private List<NewZoneRateEntity> GetHighestRatesFromZoneMatchesSaleEntities(IEnumerable<string> matchedZones, ExistingRatesByZoneName existingRatesByZoneName)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            ExistingRatesByOwner existingRatesByOwner = new ExistingRatesByOwner();

            List<ExistingRate> effectiveExistingRates;
            foreach (string matchedzone in matchedZones)
            {
                if (existingRatesByZoneName.TryGetValue(matchedzone, out effectiveExistingRates))
                {
                    foreach (ExistingRate existingRate in effectiveExistingRates)
                    {
                        if (existingRate.RateEntity.RateTypeId != null)
                            continue;

                        SalePriceList salePriceList = salePriceListManager.GetPriceList(existingRate.RateEntity.PriceListId);
                        if (salePriceList.OwnerType == SalePriceListOwnerType.Customer)
                        {
                            CarrierAccount customer = carrierAccountManager.GetCarrierAccount(salePriceList.OwnerId);
                            if (customer.CarrierAccountSettings.ActivationStatus == ActivationStatus.Inactive)
                                continue;
                        }
                        existingRatesByOwner.TryAddValue((int)salePriceList.OwnerType, salePriceList.OwnerId, existingRate);
                    }
                }
            }

            List<NewZoneRateEntity> ratesEntities = new List<NewZoneRateEntity>();

            var e = existingRatesByOwner.GetEnumerator();
            while (e.MoveNext())
            {
                Owner owner = existingRatesByOwner.GetOwner(e.Current.Key);

                HighestRate highestRate = GetHighestRate(e.Current.Value);

                if (highestRate == null) continue;

                NewZoneRateEntity zoneRate = new NewZoneRateEntity
                {
                    OwnerId = owner.OwnerId,
                    OwnerType = owner.OwnerType,
                    CurrencyId = highestRate.CurrencyId,
                    Rate = highestRate.Value,
                    RateBED = highestRate.BED,
                    HighesRateZoneId = highestRate.ZoneId
                };
                ratesEntities.Add(zoneRate);
            }
            return ratesEntities;
        }

        private HighestRate GetHighestRate(IEnumerable<ExistingRate> existingRates)
        {
            SaleRateManager saleRateManager = new SaleRateManager();

            HighestRate highestRate = new HighestRate();
            if (existingRates == null || !existingRates.Any()) return null;

            foreach (var existingRate in existingRates)
            {
                if (existingRate.RateEntity.Rate > highestRate.Value)
                {
                    highestRate.Value = existingRate.RateEntity.Rate;
                    highestRate.CurrencyId = saleRateManager.GetCurrencyId(existingRate.RateEntity);
                    highestRate.BED = existingRate.RateEntity.BED;
                    highestRate.ZoneId = existingRate.ParentZone.ZoneId;
                }
            }
            return highestRate;
        }

        private List<NewZoneRateEntity> CreateRatesWithDefaultValue(IEnumerable<string> zoneNames, ExistingRatesByZoneName existingRatesByZoneName)
        {
            Dictionary<int, NewZoneRateEntity> defaultRates = new Dictionary<int, NewZoneRateEntity>();

            SalePriceListManager priceListManager = new SalePriceListManager();
            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SellingProductManager sellingProductManager = new SellingProductManager();
            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            BusinessEntity.Business.ConfigManager businessConfigmanager = new BusinessEntity.Business.ConfigManager();

            int systemCurrencyId = configManager.GetSystemCurrencyId();

            foreach (string zoneName in zoneNames)
            {
                List<ExistingRate> effectiveExistingRates = null;
                if (existingRatesByZoneName.TryGetValue(zoneName, out effectiveExistingRates))
                {
                    foreach (ExistingRate effectiveRate in effectiveExistingRates)
                    {
                        SalePriceList pricelist = priceListManager.GetPriceList(effectiveRate.RateEntity.PriceListId);
                        decimal defaultRate = getRoundedDefaultRateForPriceListOwner(pricelist.OwnerType, pricelist.OwnerId);

                        if (!defaultRates.ContainsKey(pricelist.OwnerId))
                        {
                            int newRateCurrencyId = (pricelist.OwnerType == SalePriceListOwnerType.SellingProduct) ? sellingProductManager.GetSellingProductCurrencyId(pricelist.OwnerId) : carrierAccountManager.GetCarrierAccountCurrencyId(pricelist.OwnerId);
                            var defaultRateConverted = currencyExchangeRateManager.ConvertValueToCurrency(defaultRate, systemCurrencyId, newRateCurrencyId, DateTime.Now);
                            NewZoneRateEntity rate = new NewZoneRateEntity
                            {
                                OwnerId = pricelist.OwnerId,
                                OwnerType = pricelist.OwnerType,
                                CurrencyId = newRateCurrencyId,
                                Rate = defaultRateConverted
                            };
                            defaultRates.Add(pricelist.OwnerId, rate);
                        }
                    }
                }
            }
            return defaultRates.Values.ToList();
        }

        private decimal getRoundedDefaultRateForPriceListOwner(SalePriceListOwnerType ownerType, int ownerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SellingProductManager sellingProductManager = new SellingProductManager();

            if (ownerType == SalePriceListOwnerType.SellingProduct)
                return sellingProductManager.GetSellingProductRoundedDefaultRate(ownerId);

            return carrierAccountManager.GetCustomerRoundedDefaultRate(ownerId);
        }

        #endregion
        private ExistingRatesByZoneName StructureEffectiveExistingRatesByZoneName(IEnumerable<ExistingRate> effectiveExistingRates)
        {
            ExistingRatesByZoneName effectiveExistingRatesByZoneName = new ExistingRatesByZoneName();
            if (effectiveExistingRates != null)
            {
                List<ExistingRate> existingRates;
                foreach (ExistingRate existingRate in effectiveExistingRates)
                {
                    if (!effectiveExistingRatesByZoneName.TryGetValue(existingRate.ParentZone.Name, out existingRates))
                    {
                        existingRates = new List<ExistingRate>();
                        effectiveExistingRatesByZoneName.Add(existingRate.ParentZone.Name, existingRates);
                    }
                    existingRates.Add(existingRate);
                }
            }
            return effectiveExistingRatesByZoneName;
        }
        private void PrepareDataForPreview(ZoneToProcess zoneToProcess, ExistingRateGroup existingRateGroup)
        {
            if (existingRateGroup == null)
                return;

            IEnumerable<NotImportedRate> notImportedNormalRates = this.GetNotImportedRatesFromExistingRatesByOwner(zoneToProcess.ZoneName, existingRateGroup.NormalRates);
            zoneToProcess.NotImportedNormalRates.AddRange(notImportedNormalRates);
        }
        private void ProcessNotImportedData(IEnumerable<NotImportedZone> notImportedZones, IEnumerable<ExistingZone> existingZones,
            ExistingRateGroupByZoneName existingRateGroupByZoneName)
        {
            CloseRatesForClosedZones(existingZones);
            FillRatesForNotImportedZones(notImportedZones, existingRateGroupByZoneName);
        }
        private void FillRatesForNotImportedZones(IEnumerable<NotImportedZone> notImportedZones, ExistingRateGroupByZoneName existingRateGroupByZoneName)
        {
            if (notImportedZones == null)
                return;

            ExistingRateGroup existingRateGroup;
            foreach (NotImportedZone notImportedZone in notImportedZones)
            {
                if (existingRateGroupByZoneName.TryGetValue(notImportedZone.ZoneName, out existingRateGroup))
                {
                    if (existingRateGroup == null)
                        continue;

                    IEnumerable<NotImportedRate> notImportedNormalRates = this.GetNotImportedRatesFromExistingRatesByOwner(notImportedZone.ZoneName, existingRateGroup.NormalRates);
                    notImportedZone.NotImportedNormalRates.AddRange(notImportedNormalRates);
                }
            }
        }
        private ExistingRateGroupByZoneName StructureExistingRatesByZoneName(IEnumerable<ExistingRate> existingRates)
        {
            ExistingRateGroupByZoneName existingRateGroupByZoneName = new ExistingRateGroupByZoneName();

            if (existingRates != null)
            {
                SalePriceListManager salePriceListManager = new SalePriceListManager();

                foreach (ExistingRate existingRate in existingRates)
                {
                    //For now we are not handling other rates in Numbering Plan process
                    if (existingRate.RateEntity.RateTypeId != null)
                        continue;

                    string zoneName = existingRate.ParentZone.Name;
                    ExistingRateGroup existingRateGroup = null;
                    if (!existingRateGroupByZoneName.TryGetValue(zoneName, out existingRateGroup))
                    {
                        existingRateGroup = new ExistingRateGroup();
                        existingRateGroup.ZoneName = zoneName;
                        existingRateGroupByZoneName.Add(zoneName, existingRateGroup);
                    }

                    SalePriceList salePriceList = salePriceListManager.GetPriceList(existingRate.RateEntity.PriceListId);
                    existingRateGroup.NormalRates.TryAddValue((int)salePriceList.OwnerType, salePriceList.OwnerId, existingRate);
                }
            }

            return existingRateGroupByZoneName;
        }
        private int GetOwnerCurreny(int ownerId, SalePriceListOwnerType ownerType)
        {
            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                SellingProductManager sellingProductManager = new SellingProductManager();
                return sellingProductManager.GetSellingProductCurrencyId(ownerId);
            }

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            return carrierAccountManager.GetCarrierAccountCurrencyId(ownerId);
        }
        private void CloseRatesForClosedZones(IEnumerable<ExistingZone> existingZones)
        {
            foreach (var existingZone in existingZones)
            {
                if (existingZone.ChangedZone != null)
                {
                    DateTime zoneEED = existingZone.ChangedZone.EED;
                    if (existingZone.ExistingRates != null)
                    {
                        foreach (var existingRate in existingZone.ExistingRates)
                        {
                            DateTime? rateEED = existingRate.EED;
                            if (rateEED.VRGreaterThan(zoneEED))
                            {
                                if (existingRate.ChangedRate == null)
                                {
                                    existingRate.ChangedRate = new ChangedRate
                                    {
                                        EntityId = existingRate.RateEntity.SaleRateId
                                    };
                                }
                                DateTime rateBED = existingRate.RateEntity.BED;
                                existingRate.ChangedRate.EED = zoneEED > rateBED ? zoneEED : rateBED;
                            }
                        }
                    }
                }
            }
        }
        private Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> StructureZonesByType(IEnumerable<ExistingZone> existingZones, SaleAreaSettingsData saleAreaSettingsData)
        {
            Dictionary<long, ExistingZone> fixedExistingZonesById = new Dictionary<long, ExistingZone>();
            Dictionary<long, ExistingZone> mobileExistingZonesById = new Dictionary<long, ExistingZone>();
            ExistingZone dictionaryExistingZone;
            foreach (ExistingZone existingZone in existingZones)
            {
                if (!existingZone.IsEffectiveOrFuture(DateTime.Today))
                    continue;

                if (GetSaleZoneType(existingZone.Name, saleAreaSettingsData) == SaleZoneTypeEnum.Fixed)
                {
                    dictionaryExistingZone = fixedExistingZonesById.GetRecord(existingZone.ZoneId);
                    if (dictionaryExistingZone == null || existingZone.BED > dictionaryExistingZone.BED)
                        fixedExistingZonesById.Add(existingZone.ZoneId, existingZone);
                }

                else
                {
                    dictionaryExistingZone = mobileExistingZonesById.GetRecord(existingZone.ZoneId);
                    if (dictionaryExistingZone == null || existingZone.BED > dictionaryExistingZone.BED)
                        mobileExistingZonesById.Add(existingZone.ZoneId, existingZone);
                }
            }
            Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType = new Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>>();
            zonesByType.Add(SaleZoneTypeEnum.Fixed, fixedExistingZonesById.Values.ToList());
            zonesByType.Add(SaleZoneTypeEnum.Mobile, mobileExistingZonesById.Values.ToList());

            return zonesByType;
        }
        private SaleZoneTypeEnum GetSaleZoneType(string zoneName, SaleAreaSettingsData saleAreaSettingsData)
        {
            if (saleAreaSettingsData != null && saleAreaSettingsData.MobileKeywords.Select(item => item.ToLower()).Any(zoneName.ToLower().Contains))
                return SaleZoneTypeEnum.Mobile;

            return SaleZoneTypeEnum.Fixed;
        }
        private IEnumerable<NotImportedRate> GetNotImportedRatesFromExistingRatesByOwner(string zoneName, ExistingRatesByOwner existingRatesByOwner)
        {
            List<NotImportedRate> notImportedRates = new List<NotImportedRate>();

            var e = existingRatesByOwner.GetEnumerator();
            while (e.MoveNext())
            {
                Owner owner = existingRatesByOwner.GetOwner(e.Current.Key);
                NotImportedRate notImportedNormalRate = this.GetNotImportedRate(zoneName, owner, e.Current.Value, false);
                if (notImportedNormalRate != null)
                    notImportedRates.Add(notImportedNormalRate);
            }

            return notImportedRates;
        }
        private NotImportedRate GetNotImportedRate(string zoneName, Owner owner, List<ExistingRate> existingRates, bool hasChanged)
        {
            ExistingRate lastElement = GetLastExistingRateFromConnectedExistingRates(existingRates);
            if (lastElement == null)
                return null;
            SaleRateManager saleRateManager = new SaleRateManager();

            return new NotImportedRate()
            {
                ZoneName = zoneName,
                OwnerType = owner.OwnerType,
                OwnerId = owner.OwnerId,
                BED = lastElement.BED,
                EED = lastElement.EED,
                Rate = lastElement.RateEntity.Rate,
                RateTypeId = lastElement.RateEntity.RateTypeId,
                HasChanged = hasChanged,
                CurrencyId = saleRateManager.GetCurrencyId(lastElement.RateEntity)
            };
        }
        private ExistingRate GetLastExistingRateFromConnectedExistingRates(List<ExistingRate> existingRates)
        {
            List<ExistingRate> connectedExistingRates = existingRates.GetConnectedEntities(DateTime.Today);
            if (connectedExistingRates == null)
                return null;

            return connectedExistingRates.Last();
        }
    }
}
