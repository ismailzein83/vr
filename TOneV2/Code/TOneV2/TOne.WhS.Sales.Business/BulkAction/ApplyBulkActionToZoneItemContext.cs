using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplyBulkActionToZoneItemContext : IApplyBulkActionToZoneItemContext
    {
        #region Fields / Constructors

        private Func<Dictionary<long, ZoneItem>> _getContextZoneItems;

        private IEnumerable<CostCalculationMethod> _costCalculationMethods;

        private Func<long, SaleEntityZoneRoutingProduct> _getSellingProductZoneRoutingProduct;
        private SaleEntityZoneRateLocator SaleEntityZoneRateLocator;

        private Func<decimal, decimal> _getRoundedRate;

        private Func<DateTime, SaleEntityZoneRateLocator> _getCustomerZoneRateLocator;
        public ApplyBulkActionToZoneItemContext(Func<Dictionary<long, ZoneItem>> getContextZoneItems, IEnumerable<CostCalculationMethod> costCalculationMethods, Func<long, SaleEntityZoneRoutingProduct> getSellingProductZoneRoutingProduct, Func<decimal, decimal> getRoundedRate, SaleEntityZoneRateLocator saleEntityZoneRateLocator
            , Func<DateTime, SaleEntityZoneRateLocator> getCustomerZoneRateLocator)
        {
            _getContextZoneItems = getContextZoneItems;
            _costCalculationMethods = costCalculationMethods;
            _getSellingProductZoneRoutingProduct = getSellingProductZoneRoutingProduct;
            _getRoundedRate = getRoundedRate;
            SaleEntityZoneRateLocator = saleEntityZoneRateLocator;
            _getCustomerZoneRateLocator = getCustomerZoneRateLocator;
        }

        #endregion
        public object CustomObject { get; set; }
        public int OwnerId { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public ZoneItem ZoneItem { get; set; }

        public ZoneChanges ZoneDraft { get; set; }

        public int NewRateDayOffset { get; set; }
        public int IncreasedRateDayOffset { get; set; }
        public int DecreasedRateDayOffset { get; set; }

        public ZoneItem GetContextZoneItem(long zoneId)
        {
            Dictionary<long, ZoneItem> zoneItemsByZone = _getContextZoneItems();
            return (zoneItemsByZone != null) ? zoneItemsByZone.GetRecord(zoneId) : null;
        }

        public int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId)
        {
            return UtilitiesManager.GetCostCalculationMethodIndex(_costCalculationMethods, costCalculationMethodConfigId);
        }

        public SaleEntityZoneRoutingProduct GetSellingProductZoneRoutingProduct(long zoneId)
        {
            return _getSellingProductZoneRoutingProduct(zoneId);
        }

        public decimal GetRoundedRate(decimal rate)
        {
            return _getRoundedRate(rate);
        }

        public SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long zoneId)
        {
            return SaleEntityZoneRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);
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
    }
    public class PreApplyBulkActionToZoneItemContext : IPreApplyBulkActionToZoneItemContext
    {
        public Dictionary<int, DateTime> AdditionalCountryBEDsByCountryId { get; set; }
    }
}
