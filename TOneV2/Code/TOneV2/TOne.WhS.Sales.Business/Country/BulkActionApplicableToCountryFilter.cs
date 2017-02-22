using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class BulkActionApplicableToCountryFilter : ICountryFilter
    {
        public BulkActionType BulkAction { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public bool IsExcluded(ICountryFilterContext context)
        {
            if (context.CustomObject == null)
                context.CustomObject = new CustomObject(OwnerType, OwnerId);

            var customObject = context.CustomObject as CustomObject;

            var bulkActionApplicableToCountryContext = new BulkActionApplicableToCountryContext(customObject.GetSellingProductZoneRate, customObject.GetCustomerZoneRate, customObject.GetRateBED)
            {
                Country = context.Country,
                OwnerSellingNumberPlanId = customObject.OwnerSellingNumberPlanId,
                OwnerType = OwnerType,
                OwnerId = OwnerId,
                ZoneDraftsByZoneId = customObject.ZoneDraftsByZoneId
            };

            return !BulkAction.IsApplicableToCountry(bulkActionApplicableToCountryContext);
        }

        #region Private Classes

        private class CustomObject
        {
            #region Fields / Constructors

            private SaleEntityZoneRateLocator _currentRateLocator;
            private SaleEntityZoneRateLocator _futureRateLocator;

            private DateTime _newRateBED;
            private DateTime _increasedRateBED;
            private DateTime _decreasedRateBED;

            public CustomObject(SalePriceListOwnerType ownerType, int ownerId)
            {
                OwnerSellingNumberPlanId = new RatePlanManager().GetOwnerSellingNumberPlanId(ownerType, ownerId);

                _currentRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(DateTime.Today));
                _futureRateLocator = new SaleEntityZoneRateLocator(new FutureSaleRateReadWithCache());

                SetZoneDraftsByZoneId(ownerType, ownerId);
                SetRateBEDs();
            }

            #endregion

            public int OwnerSellingNumberPlanId { get; set; }

            public Dictionary<long, ZoneChanges> ZoneDraftsByZoneId { get; set; }

            public SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long saleZoneId, bool getFutureRate)
            {
                return (getFutureRate) ? _futureRateLocator.GetSellingProductZoneRate(sellingProductId, saleZoneId) : _currentRateLocator.GetSellingProductZoneRate(sellingProductId, saleZoneId);
            }

            public SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long saleZoneId, bool getFutureRate)
            {
                return (getFutureRate) ? _futureRateLocator.GetCustomerZoneRate(customerId, sellingProductId, saleZoneId) : _currentRateLocator.GetCustomerZoneRate(customerId, sellingProductId, saleZoneId);
            }

            public DateTime GetRateBED(decimal? currentRateValue, decimal newRateValue)
            {
                return UtilitiesManager.GetDraftRateBED(currentRateValue, newRateValue, _newRateBED, _increasedRateBED, _decreasedRateBED);
            }

            #region Private Methods

            private void SetZoneDraftsByZoneId(SalePriceListOwnerType ownerType, int ownerId)
            {
                ZoneDraftsByZoneId = new Dictionary<long, ZoneChanges>();
                Changes draft = new RatePlanDraftManager().GetDraft(ownerType, ownerId);

                if (draft != null && draft.ZoneChanges != null)
                {
                    foreach (ZoneChanges zoneDraft in draft.ZoneChanges)
                    {
                        if (!ZoneDraftsByZoneId.ContainsKey(zoneDraft.ZoneId))
                            ZoneDraftsByZoneId.Add(zoneDraft.ZoneId, zoneDraft);
                    }
                }
            }

            private void SetRateBEDs()
            {
                UtilitiesManager.SetDraftRateBEDs(out _newRateBED, out _increasedRateBED, out _decreasedRateBED);
            }

            #endregion
        }

        #endregion
    }
}
