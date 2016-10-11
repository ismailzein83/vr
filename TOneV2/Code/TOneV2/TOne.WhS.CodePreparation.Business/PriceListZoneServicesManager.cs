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
    public class PriceListZoneServicesManager
    {
        public void ProcessCountryZonesServices(IProcessCountryZonesServicesContext context)
        {
            ProcessCountryZonesServices(context.ExistingZones);
            context.ChangedZonesServices = context.ExistingZonesServices.Where(itm => itm.ChangedZoneServices != null).Select(itm => itm.ChangedZoneServices);
        }

        private void ProcessCountryZonesServices(IEnumerable<ExistingZone> existingZones)
        {
            ProcessNotImportedData(existingZones);
        }

        private void ProcessNotImportedData(IEnumerable<ExistingZone> existingZones)
        {
            CloseServicesForClosedZones(existingZones);
        }

        private void CloseServicesForClosedZones(IEnumerable<ExistingZone> existingZones)
        {
            foreach (var existingZone in existingZones)
            {
                if (existingZone.ChangedZone != null)
                {
                    DateTime zoneEED = existingZone.ChangedZone.EED;
                    if (existingZone.ExistingZonesServices != null)
                    {
                        foreach (var existingZoneServices in existingZone.ExistingZonesServices)
                        {
                            DateTime? rateEED = existingZoneServices.EED;
                            if (rateEED.VRGreaterThan(zoneEED))
                            {
                                if (existingZoneServices.ChangedZoneServices == null)
                                {
                                    existingZoneServices.ChangedZoneServices = new ChangedZoneServices
                                    {
                                        EntityId = existingZoneServices.ZoneServiceEntity.SaleEntityServiceId
                                    };
                                }
                                DateTime rateBED = existingZoneServices.ZoneServiceEntity.BED;
                                existingZoneServices.ChangedZoneServices.EED = zoneEED > rateBED ? zoneEED : rateBED;
                            }
                        }
                    }
                }
            }
        }

    }
}
