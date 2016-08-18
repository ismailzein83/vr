using System;
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
            IEnumerable<ZoneToProcess> newZones = context.ZonesToProcess.FindAllRecords(item => item.ChangeType == ZoneChangeType.New);

            ProcessCountryRates(newZones, context.ExistingZones, salePriceListsByOwner, context.EffectiveDate, context.SellingNumberPlanId);

            context.NewRates = context.ZonesToProcess.SelectMany(item => item.RatesToAdd).SelectMany(itm => itm.AddedRates);

            context.ChangedRates = context.ExistingRates.Where(itm => itm.ChangedRate != null).Select(itm => itm.ChangedRate);
        }

        private void ProcessCountryRates(IEnumerable<ZoneToProcess> zonesToProcess, IEnumerable<ExistingZone> existingZones, SalePriceListsByOwner salePriceListsToAdd, DateTime effectiveDate, int sellingNumberPlanId)
        {
            SettingManager settingManager = new SettingManager();
            SaleAreaSettingsData saleAreaSettingsData = settingManager.GetSetting<SaleAreaSettingsData>(TOne.WhS.BusinessEntity.Business.Constants.SaleAreaSettings);

            List<ExistingZone> fixedExistingZones = new List<ExistingZone>();
            List<ExistingZone> mobileExistingZones = new List<ExistingZone>();

            GetFixedAndMobileZones(existingZones, saleAreaSettingsData, fixedExistingZones, mobileExistingZones);

            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                List<ExistingZone> matchedExistingZones = new List<ExistingZone>();
                ExistingRatesByOwner existingRatesByOwner = new ExistingRatesByOwner();

                SaleZoneTypeEnum saleZoneType = this.GetSaleZoneType(zoneToProcess.ZoneName, saleAreaSettingsData);

                if (saleZoneType == SaleZoneTypeEnum.Fixed)
                {
                    GetMatchedExistingZonesByFixedZones(zoneToProcess, fixedExistingZones, matchedExistingZones);
                    GenerateNewRatesAndPriceListsByMatchedZones(matchedExistingZones, zoneToProcess, salePriceListsToAdd, effectiveDate, existingRatesByOwner);
                }
                else if (saleZoneType == SaleZoneTypeEnum.Mobile)
                    GetMatchedExistingZonesByMobileZones(zoneToProcess, mobileExistingZones, fixedExistingZones, matchedExistingZones, salePriceListsToAdd, effectiveDate, existingRatesByOwner, saleAreaSettingsData, sellingNumberPlanId);

            }

            CloseRatesForClosedZones(existingZones);

        }

        private void GetFixedAndMobileZones(IEnumerable<ExistingZone> existingZones, SaleAreaSettingsData saleAreaSettingsData, List<ExistingZone> fixedExistingZones, List<ExistingZone> mobileExistingZones)
        {
            foreach (ExistingZone existingZone in existingZones)
            {
                if (GetSaleZoneType(existingZone.Name, saleAreaSettingsData) == SaleZoneTypeEnum.Fixed)
                    fixedExistingZones.Add(existingZone);
                else
                    mobileExistingZones.Add(existingZone);
            }
        }

        private void GetMatchedExistingZonesByFixedZones(ZoneToProcess zoneToProcess, List<ExistingZone> fixedExistingZones, List<ExistingZone> matchedExistingZones)
        {
            if (fixedExistingZones.Count() > 0)
            {
                GetMatchedExistingZones(zoneToProcess.CodesToAdd, fixedExistingZones, matchedExistingZones);
            }
        }

        private void GetMatchedExistingZonesByMobileZones(ZoneToProcess zoneToProcess, List<ExistingZone> mobileExistingZones, List<ExistingZone> fixedExistingZones, List<ExistingZone> matchedExistingZones, SalePriceListsByOwner salePriceListsToAdd, DateTime effectiveDate, ExistingRatesByOwner existingRatesByOwner,
            SaleAreaSettingsData saleAreaSettingsData, int sellingNumberPlanId)
        {
            if (mobileExistingZones.Count() > 0)
            {
                GetMatchedExistingZones(zoneToProcess.CodesToAdd, mobileExistingZones, matchedExistingZones);
            }
            else if (fixedExistingZones.Count() > 0)
                GenerateNewRatesAndPriceListsBySellingProducts(matchedExistingZones, zoneToProcess, salePriceListsToAdd, effectiveDate, existingRatesByOwner, saleAreaSettingsData, sellingNumberPlanId);
        }
        private void GenerateNewRatesAndPriceListsByMatchedZones(List<ExistingZone> matchedExistingZones, ZoneToProcess zoneToProcess,
            SalePriceListsByOwner salePriceListsToAdd, DateTime effectiveDate, ExistingRatesByOwner existingRatesByOwner)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();

            foreach (ExistingZone existingZone in matchedExistingZones)
            {
                foreach (ExistingRate existingRate in existingZone.ExistingRates)
                {
                    SalePriceList salePriceList = salePriceListManager.GetPriceList(existingRate.RateEntity.PriceListId);
                    existingRatesByOwner.TryAddValue((int)salePriceList.OwnerType, salePriceList.OwnerId, existingRate);
                }
            }

            PreparePriceListsAndRatesToAdd(existingRatesByOwner, zoneToProcess, salePriceListsToAdd, effectiveDate);
            PrepareAddedRates(zoneToProcess);
        }

        private void GenerateNewRatesAndPriceListsBySellingProducts(List<ExistingZone> matchedExistingZones, ZoneToProcess zoneToProcess, SalePriceListsByOwner salePriceListsToAdd,
            DateTime effectiveDate, ExistingRatesByOwner existingRatesByOwner, SaleAreaSettingsData saleAreaSettingsData, int sellingNumberPlanId)
        {

            SellingProductManager sellingProductManager = new SellingProductManager();
            IEnumerable<SellingProduct> sellingProducts = sellingProductManager.GetSellingProductsBySellingNumberPlan(sellingNumberPlanId);

            CurrencyManager currencyManager = new CurrencyManager();
            Vanrise.Entities.Currency systemCurrency = currencyManager.GetSystemCurrency();

            foreach (SellingProduct sellingProduct in sellingProducts)
            {
                PriceListToAdd priceListToAdd = new PriceListToAdd()
                {
                    OwnerId = sellingProduct.SellingProductId,
                    OwnerType = SalePriceListOwnerType.SellingProduct,
                    EffectiveOn = effectiveDate,
                    CurrencyId = systemCurrency.CurrencyId
                };

                priceListToAdd = salePriceListsToAdd.TryAddValue(priceListToAdd);

                zoneToProcess.RatesToAdd.Add(new RateToAdd()
                {
                    PriceListToAdd = priceListToAdd,
                    Rate = saleAreaSettingsData.DefaultRate,
                    ZoneName = zoneToProcess.ZoneName
                });
            }
            PrepareAddedRates(zoneToProcess);
        }

        private void PreparePriceListsAndRatesToAdd(ExistingRatesByOwner existingRatesByOwner, ZoneToProcess zoneToProcess, SalePriceListsByOwner salePriceListsToAdd, DateTime effectiveDate)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            Vanrise.Entities.Currency systemCurrency = currencyManager.GetSystemCurrency();
            List<ExistingRate> existingRates;
            foreach (Owner owner in existingRatesByOwner.GetOwners())
            {
                PriceListToAdd priceListToAdd = new PriceListToAdd()
                    {
                        OwnerId = owner.OwnerId,
                        OwnerType = (SalePriceListOwnerType)owner.OwnerType,
                        EffectiveOn = effectiveDate,
                        CurrencyId = systemCurrency.CurrencyId
                    };

                priceListToAdd = salePriceListsToAdd.TryAddValue(priceListToAdd);

                if (existingRatesByOwner.TryGetValue(owner.OwnerType, owner.OwnerId, out existingRates))
                {
                    zoneToProcess.RatesToAdd.Add(new RateToAdd()
                    {
                        PriceListToAdd = priceListToAdd,
                        Rate = existingRates.Select(item => item.RateEntity.NormalRate).Max(),
                        ZoneName = zoneToProcess.ZoneName
                    });
                }
            }
        }

        private void PrepareAddedRates(ZoneToProcess zoneToProcess)
        {

            foreach (AddedZone addedZone in zoneToProcess.AddedZones)
            {
                foreach (RateToAdd rateToAdd in zoneToProcess.RatesToAdd)
                {
                    rateToAdd.AddedRates.Add(new AddedRate()
                    {
                        BED = addedZone.BED,
                        EED = addedZone.EED,
                        PriceListToAdd = rateToAdd.PriceListToAdd,
                        NoramlRate = rateToAdd.Rate,
                        AddedZone = addedZone
                    });
                }
            }
        }

        private void GetMatchedExistingZones(IEnumerable<CodeToAdd> codesToAdd, IEnumerable<ExistingZone> existingZones, List<ExistingZone> matchedExistingZones)
        {
            List<SaleCode> saleCodes = new List<SaleCode>();

            foreach (ExistingZone existingZone in existingZones)
            {
                saleCodes.AddRange(existingZone.ExistingCodes.Select(item => item.CodeEntity));
            }

            CodeIterator<SaleCode> codeIterator = new CodeIterator<SaleCode>(saleCodes);
            foreach (CodeToAdd codeToAdd in codesToAdd)
            {
                SaleCode matchedCode = codeIterator.GetLongestMatch(codeToAdd.Code);
                if (matchedCode != null)
                {
                    ExistingZone existingZone = existingZones.FindRecord(item => item.ZoneId == matchedCode.ZoneId);
                    if (!matchedExistingZones.Contains(existingZone))
                        matchedExistingZones.Add(existingZone);
                }
            }

            //Fill matchedZones by Fixed or Mobile zones when there is no matched zones.
            if (matchedExistingZones.Count() == 0)
                matchedExistingZones.AddRange(existingZones);

        }

        private SaleZoneTypeEnum GetSaleZoneType(string zoneName, SaleAreaSettingsData saleAreaSettingsData)
        {
            if (saleAreaSettingsData != null && saleAreaSettingsData.MobileKeywords.Select(item => item.ToLower()).Any(zoneName.ToLower().Contains))
                return SaleZoneTypeEnum.Mobile;

            return SaleZoneTypeEnum.Fixed;
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
    }
}
