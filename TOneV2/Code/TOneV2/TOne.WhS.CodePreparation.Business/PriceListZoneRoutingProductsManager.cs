using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.CodePreparation.Business
{
    public class PriceListZoneRoutingProductsManager
    {
        public void ProcessCountryZonesRoutingProducts(IProcessCountryZonesRoutingProductsContext context)
        {
            ProcessCountryZonesRoutingProducts(context.ExistingZones);
            context.ChangedZonesRoutingProducts = context.ExistingZonesRoutingProducts.Where(itm => itm.ChangedZoneRoutingProducts != null).Select(itm => itm.ChangedZoneRoutingProducts);
        }

        private void ProcessCountryZonesRoutingProducts(IEnumerable<ExistingZone> existingZones)
        {
            ProcessNotImportedData(existingZones);
        }

        private void ProcessNotImportedData(IEnumerable<ExistingZone> existingZones)
        {
            CloseRoutingProductsForClosedZones(existingZones);
        }

        private void CloseRoutingProductsForClosedZones(IEnumerable<ExistingZone> existingZones)
        {
            foreach (var existingZone in existingZones)
            {
                if (existingZone.ChangedZone != null)
                {
                    DateTime zoneEED = existingZone.ChangedZone.EED;
                    if (existingZone.ExistingZonesRoutingProducts != null)
                    {
                        foreach (var existingZoneRoutingProducts in existingZone.ExistingZonesRoutingProducts)
                        {
                            DateTime? rateEED = existingZoneRoutingProducts.EED;
                            if (rateEED.VRGreaterThan(zoneEED))
                            {
                                if (existingZoneRoutingProducts.ChangedZoneRoutingProducts == null)
                                {
                                    existingZoneRoutingProducts.ChangedZoneRoutingProducts = new ChangedZoneRoutingProducts
                                    {
                                        EntityId = existingZoneRoutingProducts.ZoneRoutingProductEntity.SaleEntityRoutingProductId
                                    };
                                }
                                DateTime rateBED = existingZoneRoutingProducts.ZoneRoutingProductEntity.BED;
                                existingZoneRoutingProducts.ChangedZoneRoutingProducts.EED = zoneEED > rateBED ? zoneEED : rateBED;
                            }
                        }
                    }
                }
            }
        }

    }
}
