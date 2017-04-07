using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common.Business;

namespace TOne.WhS.CodePreparation.Business
{
    public class MobileZoneRateLocator : NewZoneRateLocator
    {
        public MobileZoneRateLocator(int sellingNumberPlanId)
            : base(sellingNumberPlanId)
        {

        }

        public override IEnumerable<NewZoneRateEntity> GetRates(IEnumerable<CodeToAdd> codes, Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType,
            ExistingRatesByZoneName existingRatesByZoneName)
        {
            if (zonesByType[SaleZoneTypeEnum.Mobile].Count() == 0)
            {
                if (zonesByType[SaleZoneTypeEnum.Fixed].Count() == 0)
                {
                    return null;
                }
                else
                {
                    IEnumerable<string> fixedZoneNames = zonesByType[SaleZoneTypeEnum.Fixed].Select(x => x.Name);
                    return this.CreateRatesWithDefaultValueForAllSellingProducts(fixedZoneNames, existingRatesByZoneName);
                }
            }
            else
            {
                List<ExistingZone> matchedZones = base.GetMatchedExistingZones(codes, zonesByType[SaleZoneTypeEnum.Mobile]);

                if (matchedZones.Count == 0)
                    matchedZones.AddRange(zonesByType[SaleZoneTypeEnum.Mobile]);

                return base.GetHighestRatesFromZoneMatchesSaleEntities(matchedZones, existingRatesByZoneName);
            }
        }

        private List<NewZoneRateEntity> CreateRatesWithDefaultValueForAllSellingProducts(IEnumerable<string> fixedZoneNames, ExistingRatesByZoneName existingRatesByZoneName)
        {
            Dictionary<int, NewZoneRateEntity> defaultRatesBySellingProductId = new Dictionary<int, NewZoneRateEntity>();

            SalePriceListManager priceListManager = new SalePriceListManager();
            foreach (string zoneName in fixedZoneNames)
            {
                List<ExistingRate> effectiveExistingRates = null;
                if(existingRatesByZoneName.TryGetValue(zoneName, out effectiveExistingRates))
                {
                    foreach (ExistingRate effectiveRate in effectiveExistingRates)
                    {
                        SalePriceList pricelist = priceListManager.GetPriceList(effectiveRate.RateEntity.PriceListId);
                        if (pricelist.OwnerType == SalePriceListOwnerType.SellingProduct && !defaultRatesBySellingProductId.ContainsKey(pricelist.OwnerId))
                        {
                            int sellingProductCurrencyId = base.GetCurrencyForNewRate(pricelist.OwnerId, SalePriceListOwnerType.SellingProduct);
                            NewZoneRateEntity rate = new NewZoneRateEntity()
                            {
                                OwnerId = pricelist.OwnerId,
                                OwnerType = SalePriceListOwnerType.SellingProduct,
                                CurrencyId = sellingProductCurrencyId,
                                //TODO: make sure to convert from default rate currency to selling product currency later
                                Rate = base.SaleAreaSettings.DefaultRate
                            };

                            defaultRatesBySellingProductId.Add(pricelist.OwnerId, rate);
                        }
                    }
                }
            }

            return defaultRatesBySellingProductId.Values.ToList();
        }
    }
}
