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
		private Func<Dictionary<long, ZoneItem>> _getContextZoneItems;

		private IEnumerable<CostCalculationMethod> _costCalculationMethods;

		public ApplyBulkActionToZoneItemContext(Func<Dictionary<long, ZoneItem>> getContextZoneItems, IEnumerable<CostCalculationMethod> costCalculationMethods)
		{
			_getContextZoneItems = getContextZoneItems;
			_costCalculationMethods = costCalculationMethods;
		}

		public ZoneItem ZoneItem { get; set; }

		public ZoneChanges ZoneDraft { get; set; }

		public ZoneItem GetContextZoneItem(long zoneId)
		{
			Dictionary<long, ZoneItem> zoneItemsByZone = _getContextZoneItems();
			return (zoneItemsByZone != null) ? zoneItemsByZone.GetRecord(zoneId) : null;
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
	}
}
