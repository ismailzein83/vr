using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplyBulkActionToZoneDraftContext : IApplyBulkActionToZoneDraftContext
    {
        #region Fields / Constructors

        private Dictionary<long, ZoneItem> _zoneItemsByZoneId;

        private Func<IEnumerable<ZoneItem>> _buildZoneItems;

        private IEnumerable<CostCalculationMethod> _costCalculationMethods;

        private Func<decimal, decimal> _getRoundedRate;

        public ApplyBulkActionToZoneDraftContext(Func<IEnumerable<ZoneItem>> buildZoneItems, IEnumerable<CostCalculationMethod> costCalculationMethods, Func<decimal, decimal> getRoundedRate)
        {
            this._buildZoneItems = buildZoneItems;
            this._costCalculationMethods = costCalculationMethods;
            _getRoundedRate = getRoundedRate;
        }

        #endregion

        public SalePriceListOwnerType OwnerType { get; set; }

        public ZoneChanges ZoneDraft { get; set; }

        public ZoneItem GetZoneItem(long zoneId)
        {
            if (_zoneItemsByZoneId == null)
            {
                if (_buildZoneItems == null)
                    throw new MissingMemberException("_buildZoneItems");

                IEnumerable<ZoneItem> zoneItems = this._buildZoneItems();
                if (zoneItems == null || zoneItems.Count() == 0)
                    throw new NullReferenceException("zoneItems");

                StructureZoneItemsByZoneId(zoneItems);
            }

            ZoneItem zoneItem = null;
            if (!this._zoneItemsByZoneId.TryGetValue(zoneId, out zoneItem))
                throw new DataIntegrityValidationException(string.Format("Missing sale zone with Id {0}", zoneId));
            return zoneItem;
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

        public decimal GetRoundedRate(decimal rate)
        {
            return _getRoundedRate(rate);
        }

        #region Private Methods

        private void StructureZoneItemsByZoneId(IEnumerable<ZoneItem> zoneItems)
        {
            _zoneItemsByZoneId = new Dictionary<long, ZoneItem>();

            foreach (ZoneItem zoneItem in zoneItems)
            {
                if (!_zoneItemsByZoneId.ContainsKey(zoneItem.ZoneId))
                    _zoneItemsByZoneId.Add(zoneItem.ZoneId, zoneItem);
            }
        }

        #endregion
    }
}
