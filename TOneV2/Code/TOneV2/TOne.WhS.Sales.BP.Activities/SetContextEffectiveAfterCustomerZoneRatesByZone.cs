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
    public class SetContextEffectiveAfterCustomerZoneRatesByZone : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<DataByZone>> DataByZone { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            RatePlanContext ratePlanContext = context.GetRatePlanContext() as RatePlanContext;

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer)
                return;

            IEnumerable<DataByZone> dataByZone = DataByZone.Get(context);

            if (dataByZone == null || dataByZone.Count() == 0)
                return;

            DateTime? minimumDate;
            IEnumerable<long> zoneIds = GetZoneIds(dataByZone, out minimumDate);

            if (zoneIds == null || zoneIds.Count() == 0)
                return;

            IEnumerable<int> customerIds = new CustomerSellingProductManager().GetEffectiveOrFutureAssignedCustomerIds(ratePlanContext.OwnerId, minimumDate.Value);

            if (customerIds == null || customerIds.Count() == 0)
                return;

            IEnumerable<SaleRate> effectiveAfterCustomerZoneRates = new SaleRateManager().GetSaleRatesEffectiveAfterByOwnersAndZones(SalePriceListOwnerType.Customer, customerIds, zoneIds, minimumDate.Value);

            if (effectiveAfterCustomerZoneRates == null || effectiveAfterCustomerZoneRates.Count() == 0)
                return;

            var effectiveAfterCustomerZoneRatesByZone = new EffectiveAfterCustomerZoneRatesByZone();

            foreach (SaleRate rate in effectiveAfterCustomerZoneRates)
            {
                Dictionary<RateTypeKey, List<SaleRate>> zoneRatesByType;

                if (!effectiveAfterCustomerZoneRatesByZone.TryGetValue(rate.ZoneId, out zoneRatesByType))
                {
                    zoneRatesByType = new Dictionary<RateTypeKey, List<SaleRate>>();
                    effectiveAfterCustomerZoneRatesByZone.Add(rate.ZoneId, zoneRatesByType);
                }

                var rateTypeKey = new RateTypeKey() { RateTypeId = rate.RateTypeId };
                List<SaleRate> ratesOfType;

                if (!zoneRatesByType.TryGetValue(rateTypeKey, out ratesOfType))
                {
                    ratesOfType = new List<SaleRate>();
                    zoneRatesByType.Add(rateTypeKey, ratesOfType);
                }

                ratesOfType.Add(rate);
            }

            ratePlanContext.EffectiveAfterCustomerZoneRatesByZone = effectiveAfterCustomerZoneRatesByZone;
        }

        #region Private Methods

        private IEnumerable<long> GetZoneIds(IEnumerable<DataByZone> dataByZone, out DateTime? minimumDate)
        {
            var zoneIds = new List<long>();
            minimumDate = null;

            foreach (DataByZone zoneData in dataByZone)
            {
                bool shouldAddZoneId = false;

                if (zoneData.NormalRateToChange != null)
                {
                    shouldAddZoneId = true;

                    if (!minimumDate.HasValue || zoneData.NormalRateToChange.BED < minimumDate.Value)
                        minimumDate = zoneData.NormalRateToChange.BED;
                }

                if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
                {
                    shouldAddZoneId = true;

                    foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                    {
                        if (!minimumDate.HasValue || otherRateToChange.BED < minimumDate.Value)
                            minimumDate = otherRateToChange.BED;
                    }
                }

                if (shouldAddZoneId)
                    zoneIds.Add(zoneData.ZoneId);
            }

            return zoneIds;
        }

        #endregion
    }
}
