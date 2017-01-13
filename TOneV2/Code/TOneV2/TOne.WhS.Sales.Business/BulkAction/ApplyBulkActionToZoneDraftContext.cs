using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplyBulkActionToZoneDraftContext : IApplyBulkActionToZoneDraftContext
    {
        #region Private Members
        
        private Dictionary<long, ZoneItem> _zoneItemsByZoneId;

        private Func<IEnumerable<ZoneItem>> _buildZoneItems;

        #endregion

        #region Ctor

        public ApplyBulkActionToZoneDraftContext(Func<IEnumerable<ZoneItem>> buildZoneItems)
        {
            this._buildZoneItems = buildZoneItems;
        }

        #endregion

        public ZoneChanges ZoneDraft { get; set; }

        public ZoneItem GetZoneItem(long zoneId)
        {
            if(_zoneItemsByZoneId == null)
            {
                if(_buildZoneItems == null)
                    throw new MissingMemberException("_buildZoneItems");

                IEnumerable<ZoneItem> zoneItems = this._buildZoneItems();
                //Structure them in the local dic
            }

            ZoneItem zoneItem = null;

            if (!this._zoneItemsByZoneId.TryGetValue(zoneId, out zoneItem))
                throw new DataIntegrityValidationException(string.Format("Missing sale zone with Id {0}", zoneId));

            return zoneItem;
        }
    }
}
