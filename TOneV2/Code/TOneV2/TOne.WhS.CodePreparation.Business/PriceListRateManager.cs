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

        private void ProcessCountryRates(IEnumerable<ZoneToProcess> zonesToProcess, IEnumerable<ExistingZone> existingZones, SalePriceListsByOwner salePriceListsByOwner, DateTime effectiveDate, int sellingNumberPlanId)
        {
            CreateRatesForNewZones(sellingNumberPlanId, zonesToProcess, existingZones, effectiveDate, salePriceListsByOwner);
            CloseRatesForClosedZones(existingZones);
        }

        private void CreateRatesForNewZones(int sellingNumberPlanId, IEnumerable<ZoneToProcess> zonesToProcess, IEnumerable<ExistingZone> existingZones, DateTime effectiveDate, SalePriceListsByOwner salePriceListsByOwner)
        {
            //If no existing zones exist, no need to perform the whole process
            if (existingZones.Count() == 0)
                return;

            SettingManager settingManager = new SettingManager();
            SaleAreaSettingsData saleAreaSettingsData = settingManager.GetSetting<SaleAreaSettingsData>(TOne.WhS.BusinessEntity.Business.Constants.SaleAreaSettings);

            Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType = StructureZonesByType(existingZones, saleAreaSettingsData);

            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                SaleZoneTypeEnum saleZoneType = this.GetSaleZoneType(zoneToProcess.ZoneName, saleAreaSettingsData);
                NewZoneRateLocator locator;
                if (saleZoneType == SaleZoneTypeEnum.Fixed)
                    locator = new FixedZoneRateLocator(sellingNumberPlanId);
                else
                    locator = new MobileZoneRateLocator(sellingNumberPlanId);

                IEnumerable<NewZoneRateEntity> rates = locator.GetRates(zoneToProcess.CodesToAdd, zonesByType);
                
                //in some case locators return null indicating that this new zone must not have new rates assigned
                if (rates == null)
                    continue;

                foreach (NewZoneRateEntity zoneRate in rates)
                {
                    PriceListToAdd priceListToAdd = new PriceListToAdd()
                    {
                        OwnerId = zoneRate.OwnerId,
                        OwnerType = zoneRate.OwnerType,
                        EffectiveOn = effectiveDate,
                        CurrencyId = zoneRate.CurrencyId
                    };

                    priceListToAdd = salePriceListsByOwner.TryAddValue(priceListToAdd);

                    RateToAdd rateToAdd = new RateToAdd()
                    {
                        PriceListToAdd = priceListToAdd,
                        Rate = zoneRate.Rate,
                        ZoneName = zoneToProcess.ZoneName
                    };

                    foreach (AddedZone addedZone in zoneToProcess.AddedZones)
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

                    zoneToProcess.RatesToAdd.Add(rateToAdd);
                }
            }
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
            List<ExistingZone> fixedExistingZones = new List<ExistingZone>();
            List<ExistingZone> mobileExistingZones = new List<ExistingZone>();

            foreach (ExistingZone existingZone in existingZones)
            {
                if (GetSaleZoneType(existingZone.Name, saleAreaSettingsData) == SaleZoneTypeEnum.Fixed)
                    fixedExistingZones.Add(existingZone);
                else
                    mobileExistingZones.Add(existingZone);
            }

            Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType = new Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>>();
            zonesByType.Add(SaleZoneTypeEnum.Fixed, fixedExistingZones);
            zonesByType.Add(SaleZoneTypeEnum.Mobile, mobileExistingZones);

            return zonesByType;
        }

        private SaleZoneTypeEnum GetSaleZoneType(string zoneName, SaleAreaSettingsData saleAreaSettingsData)
        {
            if (saleAreaSettingsData != null && saleAreaSettingsData.MobileKeywords.Select(item => item.ToLower()).Any(zoneName.ToLower().Contains))
                return SaleZoneTypeEnum.Mobile;

            return SaleZoneTypeEnum.Fixed;
        }
    }
}
