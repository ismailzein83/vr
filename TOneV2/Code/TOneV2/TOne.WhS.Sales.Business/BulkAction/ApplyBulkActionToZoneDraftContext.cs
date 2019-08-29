using System;
using System.Linq;
using Vanrise.Entities;
using TOne.WhS.Sales.Entities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplyBulkActionToZoneDraftContext : IApplyBulkActionToZoneDraftContext
    {
        #region Fields / Constructors

        private Dictionary<long, ZoneItem> _zoneItemsByZoneId;

        private Func<IEnumerable<ZoneItem>> _buildZoneItems;

        private IEnumerable<CostCalculationMethod> _costCalculationMethods;

        private Func<decimal, decimal> _getRoundedRate;

        private Func<DateTime, SaleEntityZoneRateLocator> _getCustomerZoneRateLocator;
        public ApplyBulkActionToZoneDraftContext(Func<IEnumerable<ZoneItem>> buildZoneItems, IEnumerable<CostCalculationMethod> costCalculationMethods, Func<decimal, decimal> getRoundedRate, Func<DateTime, SaleEntityZoneRateLocator> getCustomerZoneRateLocator)
        {
            this._buildZoneItems = buildZoneItems;
            this._costCalculationMethods = costCalculationMethods;
            _getRoundedRate = getRoundedRate;
            _getCustomerZoneRateLocator = getCustomerZoneRateLocator;
        }

        #endregion

        public int OwnerId { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public ZoneChanges ZoneDraft { get; set; }

        public int NewRateDayOffset { get; set; }
        public int IncreasedRateDayOffset { get; set; }
        public int DecreasedRateDayOffset { get; set; }

        public ZoneItem GetZoneItem(long zoneId)
        {
            if (_zoneItemsByZoneId == null)
            {
                if (_buildZoneItems == null)
                    throw new MissingMemberException("_buildZoneItems");
                _zoneItemsByZoneId = BulkActionUtilities.StructureContextZoneItemsByZoneId(_buildZoneItems);
            }
            return BulkActionUtilities.GetContextZoneItem(zoneId, _zoneItemsByZoneId);
        }

        public int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId)
        {
            if (_costCalculationMethods != null)
            {
                for (int i = 0; i < _costCalculationMethods.Count(); i++)
                {
                    if (_costCalculationMethods.ElementAt(i).ConfigId.Equals(costCalculationMethodConfigId))
                        return i;
                }
            }
            return null;
        }
        public SaleRate GetCustomerNormalRate(int customerId, long zoneId, DateTime effectiveDate)
        {
            int sellingProductId = new CarrierAccountManager().GetSellingProductId(customerId);
            var rateLocator = _getCustomerZoneRateLocator(effectiveDate);
            var customerRate = rateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);

            if (customerRate == null)
                throw new DataIntegrityValidationException($"Rate is null on zone with id {zoneId} for customer with id {customerId}");

            return customerRate.Rate;
        }
        public decimal GetRoundedRate(decimal rate)
        {
            return _getRoundedRate(rate);
        }
    }
}
