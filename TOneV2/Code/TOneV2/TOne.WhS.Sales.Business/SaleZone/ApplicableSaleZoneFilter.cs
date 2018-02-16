using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Business.Reader;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class ApplicableSaleZoneFilter : ISaleZoneFilter
    {
        public BulkActionType ActionType { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public bool IsExcluded(ISaleZoneFilterContext context)
        {
            if (context.SaleZone == null)
                throw new ArgumentNullException("SaleZone");

            if (context.CustomData == null)
                context.CustomData = (object)new CustomData(this.OwnerType, this.OwnerId, DateTime.Today);

            CustomData customData = context.CustomData as CustomData;

            if (customData.ClosedCountryIds.Contains(context.SaleZone.CountryId))
                return true;

            var IsActionApplicableToZoneInput = new BulkActionApplicableToZoneInput()
            {
                OwnerType = OwnerType,
                OwnerId = OwnerId,
                SaleZone = context.SaleZone,
                BulkAction = ActionType,
                Draft = customData.Draft,
                GetCurrentSellingProductZoneRP = customData.GetCurrentSellingProductZoneRP,
                GetCurrentCustomerZoneRP = customData.GetCurrentCustomerZoneRP,
                GetSellingProductZoneRate = customData.GetSellingProductZoneRate,
                GetCustomerZoneRate = customData.GetCustomerZoneRate,
                GetRateBED = customData.GetRateBED,
                CountryBEDsByCountryId = customData.CountryBEDsByCountryId,
                CountryEEDsByCountryId = customData.CountryEEDsByCountryId
            };

            return !UtilitiesManager.IsActionApplicableToZone(IsActionApplicableToZoneInput);
        }

        #region Private Classes

        private class CustomData
        {
            private SaleEntityZoneRateLocator _currentRateLocator;
            private SaleEntityZoneRateLocator _futureRateLocator;
            private SaleEntityZoneRoutingProductLocator _routingProductLocator;

            private DateTime _newRateBED;
            private DateTime _increasedRateBED;
            private DateTime _decreasedRateBED;

            private IEnumerable<RPRouteDetailByZone> _rpRouteDetails;

            public Changes Draft { get; set; }
            public IEnumerable<int> ClosedCountryIds { get; set; }
            public Dictionary<int, DateTime> CountryBEDsByCountryId { get; set; }
            public Dictionary<int, DateTime> CountryEEDsByCountryId { get; set; }

            public CustomData(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
            {
                _futureRateLocator = new SaleEntityZoneRateLocator(new FutureSaleRateReadWithCache());
                _routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(DateTime.Today));

                Changes draft = new RatePlanDraftManager().GetDraft(ownerType, ownerId);
                Draft = draft;

                if (ownerType == SalePriceListOwnerType.Customer)
                {
                    CountryBEDsByCountryId = UtilitiesManager.GetDatesByCountry(ownerId, effectiveOn, true);
                    DraftChangedCountries draftChangedCountries = (draft != null && draft.CountryChanges != null && draft.CountryChanges.ChangedCountries != null) ? draft.CountryChanges.ChangedCountries : null;
                    CountryEEDsByCountryId = UtilitiesManager.GetCountryEEDsByCountryId(ownerId, draftChangedCountries, effectiveOn);
                }

                SetRateBEDs(ownerType, ownerId);
                SetRPRouteDetails();
                SetClosedCountryIds(ownerType, ownerId, effectiveOn);

                SetCurrentRateLocator(ownerType, ownerId, effectiveOn);
            }

            public SaleEntityZoneRoutingProduct GetCurrentSellingProductZoneRP(int sellingProductId, long saleZoneId)
            {
                return _routingProductLocator.GetSellingProductZoneRoutingProduct(sellingProductId, saleZoneId);
            }

            public SaleEntityZoneRoutingProduct GetCurrentCustomerZoneRP(int customerId, int sellingProductId, long saleZoneId)
            {
                return _routingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, saleZoneId);
            }

            public SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long zoneId, bool getFutureRate)
            {
                return (getFutureRate) ? _futureRateLocator.GetSellingProductZoneRate(sellingProductId, zoneId) : _currentRateLocator.GetSellingProductZoneRate(sellingProductId, zoneId);
            }

            public SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long zoneId, bool getFutureRate)
            {
                return (getFutureRate) ? _futureRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId) : _currentRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);
            }

            public DateTime GetRateBED(decimal? currentRateValue, decimal newRateValue)
            {
                if (!currentRateValue.HasValue)
                    return _newRateBED;
                else if (currentRateValue.Value > newRateValue)
                    return _increasedRateBED;
                else if (currentRateValue.Value < newRateValue)
                    return _decreasedRateBED;
                else
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("The current Rate '{0}' is the same as the new Rate", currentRateValue.Value));
            }

            public RPRouteDetailByZone GetRPRouteDetail(long zoneId)
            {
                return _rpRouteDetails.FindRecord(x => x.SaleZoneId == zoneId);
            }

            private void SetRateBEDs(SalePriceListOwnerType ownerType, int ownerId)
            {
                var pricingSettings = TOne.WhS.Sales.Business.UtilitiesManager.GetPricingSettings(ownerType, ownerId);

                _newRateBED = DateTime.Today.AddDays(pricingSettings.NewRateDayOffset.Value);
                _increasedRateBED = DateTime.Today.AddDays(pricingSettings.IncreasedRateDayOffset.Value);
                _decreasedRateBED = DateTime.Today.AddDays(pricingSettings.DecreasedRateDayOffset.Value);
            }

            private void SetRPRouteDetails()
            {

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

            private void SetCurrentRateLocator(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
            {
                if (ownerType == SalePriceListOwnerType.SellingProduct)
                    _currentRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));
                else
                {
                    int sellingProductId = new CarrierAccountManager().GetSellingProductId(ownerId);
                    IEnumerable<SaleZone> saleZones = new RatePlanManager().GetSaleZones(ownerType, ownerId, effectiveOn, true);
                    Dictionary<long, DateTime> zoneEffectiveDatesByZoneId = UtilitiesManager.GetZoneEffectiveDatesByZoneId(saleZones, CountryBEDsByCountryId);
                    _currentRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadRPChanges(ownerId, sellingProductId, zoneEffectiveDatesByZoneId.Keys, effectiveOn, zoneEffectiveDatesByZoneId));
                }
            }
        }

        #endregion
    }
}
