using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class PriceListZoneServiceManager
    {
        public void ProcessCountryZonesServices(IProcessCountryZonesServicesContext context)
        {
            ProcessCountryZonesServices(context.ImportedZones, context.ExistingZonesServices, context.NewAndExistingZones, context.ExistingZones, context.PriceListDate, context.NotImportedZones);

            context.NewZonesServices = context.ImportedZones.FindAllRecords(item => item.ImportedZoneService != null).SelectMany(itm => itm.ImportedZoneService.NewZoneServices);
            context.ChangedZonesServices = context.ExistingZones.SelectMany(item => item.ExistingZonesServices.Where(itm => itm.ChangedZoneService != null).Select(x => x.ChangedZoneService));
        }

        private void ProcessCountryZonesServices(IEnumerable<ImportedZone> importedZones, IEnumerable<ExistingZoneService> existingZonesServices, ZonesByName newAndExistingZones,
            IEnumerable<ExistingZone> existingZones, DateTime pricelistDate, IEnumerable<NotImportedZone> notImportedZones)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            ExistingZonesServicesByZoneName existingZonesServicesByZoneName = StructureExistingZonesServicesByZoneName(existingZonesServices);
            ProcessImportedData(importedZones, newAndExistingZones, existingZonesByName, existingZonesServicesByZoneName, pricelistDate);
            ProcessNotImportedData(existingZones, notImportedZones);
        }


        #region Processing Imported Data Methods

        private void ProcessImportedData(IEnumerable<ImportedZone> importedZones, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName, ExistingZonesServicesByZoneName existingZonesServicesByZoneName, DateTime pricelistDate)
        {
            List<ExistingZoneService> existingZoneServices;
            foreach (ImportedZone importedZone in importedZones)
            {
                existingZonesServicesByZoneName.TryGetValue(importedZone.ZoneName, out existingZoneServices);
                ProcessData(importedZone, existingZoneServices, newAndExistingZones, existingZonesByName, pricelistDate);
                //PrepareDataForPreview(importedZone);
            }
        }

        private void ProcessData(ImportedZone importedZone, List<ExistingZoneService> existingZoneServices, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName, DateTime pricelistDate)
        {

            ProcessCountryZoneServices(importedZone.ImportedZoneService, existingZoneServices, newAndExistingZones, existingZonesByName);

            if (importedZone.ImportedZoneService != null && importedZone.ImportedZoneService.ServiceIds.Count == 0)
                CloseNotImportedZoneServices(existingZoneServices, pricelistDate);
        }

        private void CloseNotImportedZoneServices(List<ExistingZoneService> existingZoneServices, DateTime zoneServiceCloseDate)
        {
            foreach (var existingZoneService in existingZoneServices)
            {
                //Get max between BED and Close Date to avoid closing a zone service with EED before BED
                DateTime? closureDate = Utilities.Max(zoneServiceCloseDate, existingZoneService.BED);
                if (!existingZoneService.ZoneServiceEntity.EED.HasValue && closureDate.VRLessThan(existingZoneService.EED))
                {
                    //Only in this case closing has a meaning, otherwise no need to close the zone service
                    existingZoneService.ChangedZoneService = new ChangedZoneService
                    {
                        EntityId = existingZoneService.ZoneServiceEntity.SupplierZoneServiceId,
                        EED = closureDate.Value
                    };
                }
            }
        }


        private void PrepareDataForPreview(ImportedZone importedZone)
        {

        }

        private void ProcessCountryZoneServices(ImportedZoneService importedZoneService, List<ExistingZoneService> existingZonesServices, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName)
        {
            if (importedZoneService != null)
                ProcessImportedZoneService(importedZoneService, existingZonesServices, newAndExistingZones, existingZonesByName);
        }

        #endregion

        #region Prcessing Not Imported Data Methods

        private void ProcessNotImportedData(IEnumerable<ExistingZone> existingZones, IEnumerable<NotImportedZone> notImportedZones)
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
                        foreach (var existingZoneService in existingZone.ExistingZonesServices)
                        {
                            DateTime? zoneServiceEED = existingZoneService.EED;
                            if (zoneServiceEED.VRGreaterThan(zoneEED))
                            {
                                if (existingZoneService.ChangedZoneService == null)
                                {
                                    existingZoneService.ChangedZoneService = new ChangedZoneService
                                    {
                                        EntityId = existingZoneService.ZoneServiceEntity.SupplierZoneServiceId
                                    };
                                }
                                DateTime zoneServiceBED = existingZoneService.ZoneServiceEntity.BED;
                                existingZoneService.ChangedZoneService.EED = zoneEED > zoneServiceBED ? zoneEED : zoneServiceBED;
                            }
                        }
                    }
                }
            }
        }


        #endregion

        #region Private Methods

        private void ProcessImportedZoneService(ImportedZoneService importedZoneService, List<ExistingZoneService> matchExistingZoneServices, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName)
        {
            if (matchExistingZoneServices != null && matchExistingZoneServices.Count() > 0)
            {
                bool shouldNotAddZoneService;
                CloseExistingOverlapedZoneServices(importedZoneService, matchExistingZoneServices, out shouldNotAddZoneService);
                if (!shouldNotAddZoneService)
                {
                    importedZoneService.ChangeType = ZoneServiceChangeType.New;
                    AddImportedZoneService(importedZoneService, newAndExistingZones, existingZonesByName);
                }
            }
            else
            {
                importedZoneService.ChangeType = ZoneServiceChangeType.New;
                AddImportedZoneService(importedZoneService, newAndExistingZones, existingZonesByName);
            }
        }

        private ExistingZonesByName StructureExistingZonesByName(IEnumerable<ExistingZone> existingZones)
        {
            ExistingZonesByName existingZonesByName = new ExistingZonesByName();
            List<ExistingZone> existingZonesList = null;

            foreach (ExistingZone item in existingZones)
            {
                if (!existingZonesByName.TryGetValue(item.Name, out existingZonesList))
                {
                    existingZonesList = new List<ExistingZone>();
                    existingZonesByName.Add(item.Name, existingZonesList);
                }

                existingZonesList.Add(item);
            }

            return existingZonesByName;

        }

        private ExistingZonesServicesByZoneName StructureExistingZonesServicesByZoneName(IEnumerable<ExistingZoneService> existingZonesServices)
        {
            List<ExistingZoneService> existingZonesServicesList = null;
            ExistingZonesServicesByZoneName existingZoneServicesByZoneName = new ExistingZonesServicesByZoneName();
            if (existingZonesServices != null)
            {
                foreach (ExistingZoneService item in existingZonesServices)
                {
                    if (!existingZoneServicesByZoneName.TryGetValue(item.ParentZone.Name, out existingZonesServicesList))
                    {
                        existingZonesServicesList = new List<ExistingZoneService>();
                        existingZoneServicesByZoneName.Add(item.ParentZone.Name, existingZonesServicesList);
                    }

                    existingZonesServicesList.Add(item);
                }

            }

            return existingZoneServicesByZoneName;
        }

        private void CloseExistingOverlapedZoneServices(ImportedZoneService importedZoneService, List<ExistingZoneService> matchExistingZoneServices, out bool shouldNotAddZoneService)
        {
            shouldNotAddZoneService = false;
            foreach (var existingZoneService in matchExistingZoneServices)
            {

                if (existingZoneService.IsOverlappedWith(importedZoneService))
                {
                    if (SameZoneServices(importedZoneService, existingZoneService))
                    {
                        if (importedZoneService.EED == existingZoneService.EED)
                        {
                            shouldNotAddZoneService = true;
                            break;
                        }
                        else if (importedZoneService.EED.HasValue && importedZoneService.EED.VRLessThan(existingZoneService.EED))
                        {
                            existingZoneService.ChangedZoneService = new ChangedZoneService
                            {
                                EntityId = existingZoneService.ZoneServiceEntity.SupplierZoneServiceId,
                                EED = importedZoneService.EED.Value
                            };
                            importedZoneService.ChangedExistingZoneServices.Add(existingZoneService);
                        }

                    }
                    else
                    {
                        DateTime existingZoneServiceEED = Utilities.Max(importedZoneService.BED, existingZoneService.BED);
                        existingZoneService.ChangedZoneService = new ChangedZoneService
                        {
                            EntityId = existingZoneService.ZoneServiceEntity.SupplierZoneServiceId,
                            EED = existingZoneServiceEED
                        };
                        importedZoneService.ChangedExistingZoneServices.Add(existingZoneService);
                    }
                }
            }

        }

        private void AddImportedZoneService(ImportedZoneService importedZoneService, ZonesByName newAndExistingZones, ExistingZonesByName existingZones)
        {
            List<IZone> zones;
            if (!newAndExistingZones.TryGetValue(importedZoneService.ZoneName, out zones))
            {
                zones = new List<IZone>();
                List<ExistingZone> matchExistingZones;
                if (existingZones.TryGetValue(importedZoneService.ZoneName, out matchExistingZones))
                    zones.AddRange(matchExistingZones);
                newAndExistingZones.Add(importedZoneService.ZoneName, zones);
            }
            DateTime currentZoneServiceBED = importedZoneService.BED;
            bool shouldAddMoreZoneServices = true;
            foreach (var zone in zones.OrderBy(itm => itm.BED))
            {
                if (zone.EED.VRGreaterThan(zone.BED) && zone.EED.VRGreaterThan(currentZoneServiceBED) && importedZoneService.EED.VRGreaterThan(zone.BED))
                {
                    AddNewZoneService(importedZoneService, ref currentZoneServiceBED, zone, out shouldAddMoreZoneServices);
                    if (!shouldAddMoreZoneServices)
                        break;
                }
            }
        }

        private void AddNewZoneService(ImportedZoneService importedZoneService, ref DateTime currentZoneServiceBED, IZone zone, out bool shouldAddMoreZoneServices)
        {
            shouldAddMoreZoneServices = false;
            List<ZoneService> zoneServices = new List<ZoneService>();
            foreach (int zoneServiceId in importedZoneService.ServiceIds)
                zoneServices.Add(new ZoneService() { ServiceId = zoneServiceId });

            var newZoneService = new NewZoneService
            {
                ZoneServices = zoneServices,
                Zone = zone,
                BED = zone.BED > currentZoneServiceBED ? zone.BED : currentZoneServiceBED,
                EED = importedZoneService.EED
            };
            if (newZoneService.EED.VRGreaterThan(zone.EED))//this means that zone has EED value
            {
                newZoneService.EED = zone.EED;
                currentZoneServiceBED = newZoneService.EED.Value;
                shouldAddMoreZoneServices = true;
            }

            zone.NewZoneServices.Add(newZoneService);

            importedZoneService.NewZoneServices.Add(newZoneService);
        }

        private bool SameZoneServices(ImportedZoneService importedZoneService, ExistingZoneService existingZoneService)
        {
            return importedZoneService.BED == existingZoneService.BED
               && sameServiceIds(existingZoneService.ZoneServiceEntity.ReceivedServices, importedZoneService.ServiceIds);
            //TODO: compare CurrencyId of the Pricelists

        }


        private bool sameServiceIds(List<ZoneService> zoneServices, List<int> serviceIds)
        {
            if (!(zoneServices.Count == serviceIds.Count))
                return false;

            foreach (ZoneService zoneService in zoneServices)
            {
                if (!serviceIds.Contains(zoneService.ServiceId))
                    return false;
            }

            return true;
        }

        #endregion
    }
}
