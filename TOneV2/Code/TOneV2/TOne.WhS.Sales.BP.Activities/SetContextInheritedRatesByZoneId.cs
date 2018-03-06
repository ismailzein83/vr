using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SetContextInheritedRatesByZoneId : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<DateTime> MinimumActionDate { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            var ratePlanContext = context.GetRatePlanContext() as RatePlanContext;

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return;

            int sellingProductId = new CarrierAccountManager().GetSellingProductId(ratePlanContext.OwnerId);
            IEnumerable<long> saleZoneIds = ratePlanContext.ExistingZonesByCountry.SelectMany(x => x.Value).MapRecords(x => x.ZoneId);

            DateTime minimumActionDate = MinimumActionDate.Get(context);
            DateTime minimumDate = Utilities.Min(minimumActionDate, DateTime.Today);

            IEnumerable<SaleRate> rates = new SaleRateManager().GetExistingRatesByZoneIds(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneIds, minimumDate);
            ratePlanContext.InheritedRatesByZoneId = StructureRatesByZoneId(rates);
            ratePlanContext.InheritedRates = rates;
        }

        #region Private Methods

        private InheritedRatesByZoneId StructureRatesByZoneId(IEnumerable<SaleRate> rates)
        {
            var ratesByZoneId = new InheritedRatesByZoneId();

            if (rates != null)
            {
                foreach (SaleRate rate in rates.OrderBy(x => x.BED))
                {
                    ZoneInheritedRates zoneRates;

                    if (!ratesByZoneId.TryGetValue(rate.ZoneId, out zoneRates))
                    {
                        zoneRates = new ZoneInheritedRates();
                        ratesByZoneId.Add(rate.ZoneId, zoneRates);
                    }

                    if (!rate.RateTypeId.HasValue)
                        zoneRates.NormalRates.Add(rate);
                    else
                    {
                        List<SaleRate> otherRates;

                        if (!zoneRates.OtherRatesByRateTypeId.TryGetValue(rate.RateTypeId.Value, out otherRates))
                        {
                            otherRates = new List<SaleRate>();
                            zoneRates.OtherRatesByRateTypeId.Add(rate.RateTypeId.Value, otherRates);
                        }

                        otherRates.Add(rate);
                    }
                }
            }

            return ratesByZoneId;
        }

        #endregion
    }
}
