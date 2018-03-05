using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class BulkActionUtilities
    {
        public static Dictionary<long, ZoneItem> StructureContextZoneItemsByZoneId(Func<IEnumerable<ZoneItem>> buildContextZoneItems)
        {
            IEnumerable<ZoneItem> contextZoneItems = buildContextZoneItems();

            if (contextZoneItems == null || contextZoneItems.Count() == 0)
                throw new DataIntegrityValidationException("contextZoneItems");

            var contextZoneItemsByZoneId = new Dictionary<long, ZoneItem>();

            foreach (ZoneItem contextZoneItem in contextZoneItems)
            {
                if (!contextZoneItemsByZoneId.ContainsKey(contextZoneItem.ZoneId))
                    contextZoneItemsByZoneId.Add(contextZoneItem.ZoneId, contextZoneItem);
            }

            return contextZoneItemsByZoneId;
        }

        public static ZoneItem GetContextZoneItem(long zoneId, Dictionary<long, ZoneItem> contextZoneItemsByZoneId)
        {
            ZoneItem contextZoneItem;
            if (!contextZoneItemsByZoneId.TryGetValue(zoneId, out contextZoneItem))
                throw new DataIntegrityValidationException(string.Format("Missing sale zone with Id {0}", zoneId));
            return contextZoneItem;
        }

        public static string GetZoneNameKey(string importedZoneName)
        {
            return (importedZoneName != null) ? importedZoneName.Trim().ToLower() : null;
        }
    }
}
