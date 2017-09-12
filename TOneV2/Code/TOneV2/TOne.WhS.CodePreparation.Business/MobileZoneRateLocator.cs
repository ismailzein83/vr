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
    //TODO to remove this class because it's no longer used
    public class MobileZoneRateLocator : NewZoneRateLocator
    {
        public MobileZoneRateLocator(int sellingNumberPlanId)
            : base(sellingNumberPlanId)
        {

        }

        public override IEnumerable<NewZoneRateEntity> GetRates(IEnumerable<CodeToAdd> codes, Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType, ExistingRatesByZoneName existingRatesByZoneName)
        {
            if (!zonesByType[SaleZoneTypeEnum.Mobile].Any())
            {
                if (!zonesByType[SaleZoneTypeEnum.Fixed].Any())
                    return null;

                IEnumerable<string> fixedZoneNames = zonesByType[SaleZoneTypeEnum.Fixed].Select(x => x.Name);
                return this.CreateRatesWithDefaultValue(fixedZoneNames, existingRatesByZoneName);
            }

            List<ExistingZone> matchedZones = base.GetMatchedExistingZones(codes, zonesByType[SaleZoneTypeEnum.Mobile]);

            if (matchedZones.Count == 0)
                matchedZones.AddRange(zonesByType[SaleZoneTypeEnum.Mobile]);

            return base.GetHighestRatesFromZoneMatchesSaleEntities(matchedZones, existingRatesByZoneName);
        }

        private List<NewZoneRateEntity> CreateRatesWithDefaultValue(IEnumerable<string> fixedZoneNames, ExistingRatesByZoneName existingRatesByZoneName)
        {
            Dictionary<int, NewZoneRateEntity> defaultRates = new Dictionary<int, NewZoneRateEntity>();

            SalePriceListManager priceListManager = new SalePriceListManager();
            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();

            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            int systemCurrencyId = configManager.GetSystemCurrencyId();
            foreach (string zoneName in fixedZoneNames)
            {
                List<ExistingRate> effectiveExistingRates = null;
                if (existingRatesByZoneName.TryGetValue(zoneName, out effectiveExistingRates))
                {
                    foreach (ExistingRate effectiveRate in effectiveExistingRates)
                    {
                        SalePriceList pricelist = priceListManager.GetPriceList(effectiveRate.RateEntity.PriceListId);
                        if (!defaultRates.ContainsKey(pricelist.OwnerId))
                        {
                            int newRateCurrencyId = base.GetCurrencyForNewRate(pricelist.OwnerId, pricelist.OwnerType);
                            var sellingProductManager = new SellingProductManager();
                            var carrierAccountManager = new CarrierAccountManager();
                            NewZoneRateEntity rate = new NewZoneRateEntity
                            {
                                OwnerId = pricelist.OwnerId,
                                OwnerType = pricelist.OwnerType,
                                CurrencyId = newRateCurrencyId,
                                //TODO: make sure to convert from default rate currency to selling product currency later
                                Rate = pricelist.OwnerType == SalePriceListOwnerType.SellingProduct
                                        ? sellingProductManager.GetSellingProductDefaultRate(pricelist.OwnerId)
                                        : currencyExchangeRateManager.ConvertValueToCurrency(carrierAccountManager.GetCustomerDefaultRate(pricelist.OwnerId), systemCurrencyId, newRateCurrencyId, DateTime.Now)
                            };
                            defaultRates.Add(pricelist.OwnerId, rate);
                        }
                    }
                }
            }
            return defaultRates.Values.ToList();
        }
    }
}
