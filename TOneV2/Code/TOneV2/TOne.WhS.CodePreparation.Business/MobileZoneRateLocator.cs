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

        public override IEnumerable<NewZoneRateEntity> GetRates(IEnumerable<CodeToAdd> codes, Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType)
        {
            if (zonesByType[SaleZoneTypeEnum.Mobile].Count() == 0)
            {
                if (zonesByType[SaleZoneTypeEnum.Fixed].Count() == 0)
                    return null;
                else
                    return this.CreateRatesWithDefaultValueForAllSellingProducts();
            }
            else
            {
                List<ExistingZone> matchedZones = base.GetMatchedExistingZones(codes, zonesByType[SaleZoneTypeEnum.Mobile]);

                if (matchedZones.Count == 0)
                    matchedZones.AddRange(zonesByType[SaleZoneTypeEnum.Mobile]);

                return base.GetHighestRatesFromZoneMatchesSaleEntities(matchedZones);
            }
        }

        private List<NewZoneRateEntity> CreateRatesWithDefaultValueForAllSellingProducts()
        {
            List<NewZoneRateEntity> ratesEntities = null;

            SellingProductManager sellingProductManager = new SellingProductManager();
            IEnumerable<SellingProduct> allSellingProducts = sellingProductManager.GetSellingProductsBySellingNumberPlan(base.SellingNumberPlanId);

            if (allSellingProducts != null && allSellingProducts.Count() > 0)
            {
                ratesEntities = new List<NewZoneRateEntity>();
                foreach (SellingProduct sellingProduct in allSellingProducts)
                {
                    int sellingProductCurrencyId = base.GetCurrencyForNewRate(sellingProduct.SellingProductId, SalePriceListOwnerType.SellingProduct);
                    NewZoneRateEntity rate = new NewZoneRateEntity()
                    {
                        OwnerId = sellingProduct.SellingProductId,
                        OwnerType = SalePriceListOwnerType.SellingProduct,
                        CurrencyId = sellingProductCurrencyId,
                        //TODO: make sure to convert from default rate currency to selling product currency later
                        Rate = base.SaleAreaSettings.DefaultRate
                    };

                    ratesEntities.Add(rate);
                }
            }
            return ratesEntities;
        }
    }
}
