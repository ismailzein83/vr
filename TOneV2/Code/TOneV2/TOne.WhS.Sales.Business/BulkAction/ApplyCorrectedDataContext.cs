using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplyCorrectedDataContext : IApplyCorrectedDataContext
    {
        Func<long, ZoneChanges> _getZoneDraft;
        Func<IEnumerable<ZoneItem>> _buildContextZoneItems;
        Func<decimal, decimal> _getRoundedRate;

        Dictionary<long, ZoneItem> _contextZoneItemsByZoneId;

        public ApplyCorrectedDataContext(Func<long, ZoneChanges> getZoneDraft, Func<IEnumerable<ZoneItem>> buildContextZoneItems, Func<decimal, decimal> getRoundedRate)
        {
            _getZoneDraft = getZoneDraft;
            _buildContextZoneItems = buildContextZoneItems;
            _getRoundedRate = getRoundedRate;
        }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public BulkActionCorrectedData CorrectedData { get; set; }

        public ZoneChanges GetZoneDraft(long zoneId)
        {
            return _getZoneDraft(zoneId);
        }

        public ZoneItem GetContextZoneItem(long zoneId)
        {
            if (_contextZoneItemsByZoneId == null)
            {
                if (_buildContextZoneItems == null)
                    throw new MissingMemberException("_buildContextZoneItems");
                _contextZoneItemsByZoneId = BulkActionUtilities.StructureContextZoneItemsByZoneId(_buildContextZoneItems);
            }
            return BulkActionUtilities.GetContextZoneItem(zoneId, _contextZoneItemsByZoneId);
        }

        public decimal GetRoundedRate(decimal rateValue)
        {
            return _getRoundedRate(rateValue);
        }
    }
}
