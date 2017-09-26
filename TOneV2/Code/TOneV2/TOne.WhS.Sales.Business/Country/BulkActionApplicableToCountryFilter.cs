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
                context.CustomObject = new CustomObject(OwnerType, OwnerId, DateTime.Today);

            var customObject = context.CustomObject as CustomObject;

            if (customObject.ClosedCountryIds.Contains(context.Country.CountryId))
                return true;

            var bulkActionApplicableToCountryContext = new BulkActionApplicableToCountryContext(customObject.GetCurrentSellingProductZoneRP, customObject.GetCurrentCustomerZoneRP, customObject.GetSellingProductZoneRate, customObject.GetCustomerZoneRate, customObject.GetRateBED)
            {
                Country = context.Country,
                OwnerSellingNumberPlanId = customObject.OwnerSellingNumberPlanId,
                OwnerType = OwnerType,
                OwnerId = OwnerId,
                ZoneDraftsByZoneId = customObject.ZoneDraftsByZoneId,
                CountryBEDsByCountryId = customObject.CountryBEDsByCountryId
            };

            return !BulkAction.IsApplicableToCountry(bulkActionApplicableToCountryContext);
        }

        #region Private Classes

        private class CustomObject
        {
            #region Fields / Constructors

            private SaleEntityZoneRateLocator _currentRateLocator;
            private SaleEntityZoneRateLocator _futureRateLocator;
            private SaleEntityZoneRoutingProductLocator _routingProductLocator;

            private DateTime _newRateBED;
            private DateTime _increasedRateBED;
            private DateTime _decreasedRateBED;

            public CustomObject(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
            {
                OwnerSellingNumberPlanId = new RatePlanManager().GetOwnerSellingNumberPlanId(ownerType, ownerId);
                CountryBEDsByCountryId = (ownerType == SalePriceListOwnerType.Customer) ? UtilitiesManager.GetDatesByCountry(ownerId, effectiveOn, true) : null;

                DateTime today = DateTime.Today;
                _currentRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(today));
                _futureRateLocator = new SaleEntityZoneRateLocator(new FutureSaleRateReadWithCache());
                _routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(today));

                SetZoneDraftsByZoneId(ownerType, ownerId);
                SetRateBEDs(ownerType, ownerId);
                SetClosedCountryIds(ownerType, ownerId, effectiveOn);
            }

            #endregion

            public int OwnerSellingNumberPlanId { get; set; }

            public Dictionary<long, ZoneChanges> ZoneDraftsByZoneId { get; set; }

            public IEnumerable<int> ClosedCountryIds { get; set; }

            public Dictionary<int, DateTime> CountryBEDsByCountryId { get; set; }

            public SaleEntityZoneRoutingProduct GetCurrentSellingProductZoneRP(int sellingProductId, long saleZoneId)
            {
                return _routingProductLocator.GetSellingProductZoneRoutingProduct(sellingProductId, saleZoneId);
            }

            public SaleEntityZoneRoutingProduct GetCurrentCustomerZoneRP(int customerId, int sellingProductId, long saleZoneId)
            {
                return _routingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, saleZoneId);
            }

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

            private void SetRateBEDs(SalePriceListOwnerType ownerType, int ownerId)
            {
                UtilitiesManager.SetDraftRateBEDs(ownerType, ownerId, out _newRateBED, out _increasedRateBED, out _decreasedRateBED);
            }

            private void SetClosedCountryIds(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
            {
                if (ownerType == SalePriceListOwnerType.SellingProduct)
                    ClosedCountryIds = new List<int>();
                else
                {
                    Changes draft = new RatePlanDraftManager().GetDraft(ownerType, ownerId);
                    ClosedCountryIds = UtilitiesManager.GetClosedCountryIds(ownerId, draft, effectiveOn);
                }
            }

            #endregion
        }

        #endregion
    }
}
