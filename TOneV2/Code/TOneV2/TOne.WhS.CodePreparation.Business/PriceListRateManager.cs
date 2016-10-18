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
            ProcessCountryRates(context.ZonesToProcess, context.ExistingZones, context.ExistingRates, salePriceListsByOwner, context.NotImportedZones, context.EffectiveDate, context.SellingNumberPlanId);
            context.NewRates = context.ZonesToProcess.SelectMany(item => item.RatesToAdd).SelectMany(itm => itm.AddedRates);
            context.ChangedRates = context.ExistingRates.Where(itm => itm.ChangedRate != null).Select(itm => itm.ChangedRate);
        }

        private void ProcessCountryRates(IEnumerable<ZoneToProcess> zonesToProcess, IEnumerable<ExistingZone> existingZones, IEnumerable<ExistingRate> existingRates,
            SalePriceListsByOwner salePriceListsByOwner, IEnumerable<NotImportedZone> notImportedZones, DateTime effectiveDate, int sellingNumberPlanId)
        {
            ExistingRateGroupByZoneName existingRateGroupByZoneName = StructureExistingRatesByZoneName(existingRates);
            ProcessNotImportedData(notImportedZones, existingZones, existingRateGroupByZoneName);
            ProcessImportedData(zonesToProcess, existingZones, existingRateGroupByZoneName, salePriceListsByOwner, effectiveDate, sellingNumberPlanId);
           
        }

        private void ProcessImportedData(IEnumerable<ZoneToProcess> zonesToProcess, IEnumerable<ExistingZone> existingZones, 
            ExistingRateGroupByZoneName existingRateGroupByZoneName, SalePriceListsByOwner salePriceListsByOwner, DateTime effectiveDate, int sellingNumberPlanId)
        {
            //If no existing zones exist, no need to perform the whole process
            if (existingZones.Count() == 0)
                return;

            SettingManager settingManager = new SettingManager();
            SaleAreaSettingsData saleAreaSettingsData = settingManager.GetSetting<SaleAreaSettingsData>(TOne.WhS.BusinessEntity.Business.Constants.SaleAreaSettings);

            Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType = StructureZonesByType(existingZones, saleAreaSettingsData);

            ExistingRateGroup existingRateGroup;
            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                existingRateGroupByZoneName.TryGetValue(zoneToProcess.ZoneName, out existingRateGroup);

                if (zoneToProcess.ChangeType == ZoneChangeType.New)
                    CreateRatesForNewZones(zoneToProcess, sellingNumberPlanId, saleAreaSettingsData, zonesByType, effectiveDate, salePriceListsByOwner);
                else if (zoneToProcess.ChangeType == ZoneChangeType.Renamed)
                {
                    existingRateGroupByZoneName.TryGetValue(zoneToProcess.RecentZoneName, out existingRateGroup);
                    GenerateRatesForRenamedZoneFromOriginalZone(zoneToProcess, existingRateGroup,effectiveDate, salePriceListsByOwner);
                }
                else
                    PrepareDataForPreview(zoneToProcess, existingRateGroup);
            }
        }

        private void GenerateRatesForRenamedZoneFromOriginalZone(ZoneToProcess zoneToProcess, ExistingRateGroup existingRateGroup, DateTime effectiveDate, SalePriceListsByOwner salePriceListsByOwner)
        {
            if (existingRateGroup != null)
            {
                ExistingRatesByOwner existingRatesByOwner = new ExistingRatesByOwner();
                SaleRateManager saleRateManager = new SaleRateManager();

                var e =  existingRateGroup.NormalRates.GetEnumerator();

                while (e.MoveNext())
                {
                    ExistingRate lastExistingRate = GetLastExistingRateFromConnectedExistingRates(e.Current.Value);
                    Owner owner = existingRatesByOwner.GetOwner(e.Current.Key);
                    PriceListToAdd priceListToAdd = new PriceListToAdd()
                    {
                        OwnerId = owner.OwnerId,
                        OwnerType = owner.OwnerType,
                        EffectiveOn = effectiveDate,
                        CurrencyId = saleRateManager.GetCurrencyId(lastExistingRate.RateEntity)
                    };

                    priceListToAdd = salePriceListsByOwner.TryAddValue(priceListToAdd);

                    RateToAdd rateToAdd = new RateToAdd()
                    {
                        PriceListToAdd = priceListToAdd,
                        Rate = lastExistingRate.RateEntity.Rate,
                        ZoneName = zoneToProcess.ZoneName,
                    };

                    foreach (AddedZone addedZone in zoneToProcess.AddedZones)
                    {
                        rateToAdd.AddedRates.Add(new AddedRate()
                        {
                            BED = addedZone.BED,
                            EED = addedZone.EED,
                            PriceListToAdd = rateToAdd.PriceListToAdd,
                            NormalRate = rateToAdd.Rate,
                            AddedZone = addedZone
                        });
                    }

                    zoneToProcess.RatesToAdd.Add(rateToAdd);
                }
            }
        }

        private void GenerateRatesForRenamedZoneFromOriginalZone(ZoneToProcess zoneToProcess, DateTime effectiveDate, SalePriceListsByOwner salePriceListsByOwner)
        {
           foreach (NotImportedRate notImportedRate in zoneToProcess.NotImportedNormalRates)
            {
                PriceListToAdd priceListToAdd = new PriceListToAdd()
                {
                    OwnerId = notImportedRate.OwnerId,
                    OwnerType = notImportedRate.OwnerType,
                    EffectiveOn = effectiveDate,
                   //CurrencyId = notImportedRate.
                };

                priceListToAdd = salePriceListsByOwner.TryAddValue(priceListToAdd);

                RateToAdd rateToAdd = new RateToAdd()
                {
                    PriceListToAdd = priceListToAdd,
                    Rate = notImportedRate.Rate,
                    ZoneName = zoneToProcess.ZoneName,
                };

                foreach (AddedZone addedZone in zoneToProcess.AddedZones)
                {
                    rateToAdd.AddedRates.Add(new AddedRate()
                    {
                        BED = addedZone.BED,
                        EED = addedZone.EED,
                        PriceListToAdd = rateToAdd.PriceListToAdd,
                        NormalRate = rateToAdd.Rate,
                        AddedZone = addedZone
                    });
                }

                zoneToProcess.RatesToAdd.Add(rateToAdd);
            }
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

        private void CreateRatesForNewZones(ZoneToProcess zoneToProcess, int sellingNumberPlanId, SaleAreaSettingsData saleAreaSettingsData,
            Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType, DateTime effectiveDate, SalePriceListsByOwner salePriceListsByOwner)
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
                    return;

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
                            NormalRate = rateToAdd.Rate,
                            AddedZone = addedZone
                        });
                    }

                    zoneToProcess.RatesToAdd.Add(rateToAdd);
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

            return new NotImportedRate()
            {
                ZoneName = zoneName,
                OwnerType = owner.OwnerType,
                OwnerId = owner.OwnerId,
                BED = lastElement.BED,
                EED = lastElement.EED,
                Rate = lastElement.RateEntity.Rate,
                RateTypeId = lastElement.RateEntity.RateTypeId,
                HasChanged = hasChanged
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
