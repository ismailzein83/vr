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

namespace TOne.WhS.Sales.Business
{
    public class ApplyBulkActionToZoneItemContext : IApplyBulkActionToZoneItemContext
    {
        #region Fields / Constructors

        private Func<Dictionary<long, ZoneItem>> _getContextZoneItems;

        private IEnumerable<CostCalculationMethod> _costCalculationMethods;

        private Func<long, SaleEntityZoneRoutingProduct> _getSellingProductZoneRoutingProduct;

        public ApplyBulkActionToZoneItemContext(Func<Dictionary<long, ZoneItem>> getContextZoneItems, IEnumerable<CostCalculationMethod> costCalculationMethods, Func<long, SaleEntityZoneRoutingProduct> getSellingProductZoneRoutingProduct)
        {
            _getContextZoneItems = getContextZoneItems;
            _costCalculationMethods = costCalculationMethods;
            _getSellingProductZoneRoutingProduct = getSellingProductZoneRoutingProduct;
        }

        #endregion

        public SalePriceListOwnerType OwnerType { get; set; }

        public ZoneItem ZoneItem { get; set; }

        public ZoneChanges ZoneDraft { get; set; }

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
    }
}
