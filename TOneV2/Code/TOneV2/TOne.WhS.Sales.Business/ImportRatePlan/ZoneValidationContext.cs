using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class ZoneValidationContext : IZoneValidationContext
    {
        private Func<Dictionary<long, ZoneItem>> _getContextZoneItems;

        private IEnumerable<CostCalculationMethod> _costCalculationMethods;

        public ZoneValidationContext(Func<Dictionary<long, ZoneItem>> getContextZoneItems, IEnumerable<CostCalculationMethod> costCalculationMethods)
        {
            _getContextZoneItems = getContextZoneItems;
            _costCalculationMethods = costCalculationMethods;
        }

        public long ZoneId { get; set; }

        public ZoneItem GetContextZoneItem(long zoneId)
        {
            Dictionary<long, ZoneItem> contextZoneItems = _getContextZoneItems();
            return contextZoneItems.GetRecord(zoneId);
        }

        public int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId)
        {
            return UtilitiesManager.GetCostCalculationMethodIndex(_costCalculationMethods, costCalculationMethodConfigId);
        }

        public BulkActionValidationResult ValidationResult { get; set; }
    }
}
