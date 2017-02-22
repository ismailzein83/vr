using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
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
            {
                context.CustomData = (object)new CustomData(this.OwnerType, this.OwnerId);
            }

            CustomData customData = context.CustomData as CustomData;

            var IsActionApplicableToZoneInput = new IsActionApplicableToZoneInput()
            {
                OwnerType = OwnerType,
                OwnerId = OwnerId,
                SaleZone = context.SaleZone,
                BulkAction = ActionType,
                Draft = customData.Draft,
                GetSellingProductZoneRate = customData.GetSellingProductZoneRate,
                GetCustomerZoneRate = customData.GetCustomerZoneRate,
                GetRateBED = customData.GetRateBED
            };

            return !UtilitiesManager.IsActionApplicableToZone(IsActionApplicableToZoneInput);
        }

        private class CustomData
        {
            private SaleEntityZoneRateLocator _futureRateLocator;

            private DateTime _newRateBED;
            private DateTime _increasedRateBED;
            private DateTime _decreasedRateBED;

            private IEnumerable<RPRouteDetail> _rpRouteDetails;

            public Changes Draft { get; set; }

            public CustomData(SalePriceListOwnerType ownerType, int ownerId)
            {
                _futureRateLocator = new SaleEntityZoneRateLocator(new FutureSaleRateReadWithCache());
                Draft = new RatePlanDraftManager().GetDraft(ownerType, ownerId);
                SetRateBEDs();
                SetRPRouteDetails();
            }

            public SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long zoneId, bool getFutureRate)
            {
                return _futureRateLocator.GetSellingProductZoneRate(sellingProductId, zoneId);
            }

            public SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long zoneId, bool getFutureRate)
            {
                return _futureRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);
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

            public RPRouteDetail GetRPRouteDetail(long zoneId)
            {
                return _rpRouteDetails.FindRecord(x => x.SaleZoneId == zoneId);
            }

            private void SetRateBEDs()
            {
                var configManager = new ConfigManager();
                _newRateBED = DateTime.Today.AddDays(configManager.GetNewRateDayOffset());
                _increasedRateBED = DateTime.Today.AddDays(configManager.GetIncreasedRateDayOffset());
                _decreasedRateBED = DateTime.Today.AddDays(configManager.GetDecreasedRateDayOffset());
            }

            private void SetRPRouteDetails()
            {

            }
        }
    }
}
