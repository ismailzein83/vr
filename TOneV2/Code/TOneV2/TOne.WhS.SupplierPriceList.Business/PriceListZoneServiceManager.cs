﻿using System;
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

            context.NewZonesServices = context.ImportedZones.FindAllRecords(item => item.ImportedZoneServiceGroup != null).SelectMany(itm => itm.ImportedZoneServiceGroup.NewZoneServices);
            context.ChangedZonesServices = context.ExistingZones.SelectMany(item => item.ExistingZonesServices.Where(itm => itm.ChangedZoneService != null).Select(x => x.ChangedZoneService));
        }

        private void ProcessCountryZonesServices(IEnumerable<ImportedZone> importedZones, IEnumerable<ExistingZoneService> existingZonesServices, ZonesByName newAndExistingZones,
            IEnumerable<ExistingZone> existingZones, DateTime pricelistDate, IEnumerable<NotImportedZone> notImportedZones)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            ExistingZonesServicesByZoneName existingZonesServicesByZoneName = StructureExistingZonesServicesByZoneName(existingZonesServices);
            ProcessImportedData(importedZones, newAndExistingZones, existingZonesByName, existingZonesServicesByZoneName, pricelistDate);
            ProcessNotImportedData(existingZones, notImportedZones, existingZonesServicesByZoneName);
        }


        #region Processing Imported Data Methods

        private void ProcessImportedData(IEnumerable<ImportedZone> importedZones, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName, ExistingZonesServicesByZoneName existingZonesServicesByZoneName, DateTime pricelistDate)
        {
            List<ExistingZoneService> existingZoneServices;
            foreach (ImportedZone importedZone in importedZones)
            {
                existingZonesServicesByZoneName.TryGetValue(importedZone.ZoneName, out existingZoneServices);
                ProcessData(importedZone, existingZoneServices, newAndExistingZones, existingZonesByName, pricelistDate);
                PrepareDataForPreview(importedZone, existingZoneServices);
            }
        }

        private void PrepareDataForPreview(ImportedZone importedZone, List<ExistingZoneService> existingZoneServices)
        {
            if (importedZone.ImportedZoneServiceGroup != null)
                FillSystemZoneServicesForImportedZone(importedZone, existingZoneServices);
            if (importedZone.ImportedZoneServiceGroup == null && existingZoneServices != null)
                FillNotImportedZoneServicesWithClosedZoneServices(importedZone, existingZoneServices);
        }

        private void FillNotImportedZoneServicesWithClosedZoneServices(ImportedZone importedZone, List<ExistingZoneService> existingZoneServices)
        {
            NotImportedZoneService notImportedZoneService = this.GetNotImportedZoneService(existingZoneServices, true);
            if (notImportedZoneService != null)
                importedZone.NotImportedZoneService = notImportedZoneService;
        }

        private NotImportedZoneService GetNotImportedZoneService(List<ExistingZoneService> existingZoneServices, bool hasChanged)
        {
            ExistingZoneService lastElement = GetLastExistingZoneServiceFromConnectedExistingZoneServices(existingZoneServices);
            if (lastElement == null)
                return null;

            return new NotImportedZoneService()
            {
                BED = lastElement.BED,
                EED = lastElement.ZoneServiceEntity.EED,
                ZoneServicesIds = lastElement.ZoneServiceEntity.ReceivedServices.Select(item => item.ServiceId).ToList(),
                HasChanged = hasChanged
            };
        }

        private void FillSystemZoneServicesForImportedZone(ImportedZone importedZone, List<ExistingZoneService> existingZoneServices)
        {
            if (existingZoneServices == null)
                return;

            importedZone.ImportedZoneServiceGroup.SystemZoneService = GetSystemZoneService(existingZoneServices);
        }

        private ExistingZoneService GetSystemZoneService(List<ExistingZoneService> existingZoneServices)
        {
            return GetLastExistingZoneServiceFromConnectedExistingZoneServices(existingZoneServices);
        }

        private ExistingZoneService GetLastExistingZoneServiceFromConnectedExistingZoneServices(List<ExistingZoneService> existingZoneServices)
        {
            List<ExistingZoneService> connectedExistingZonesServices = existingZoneServices.GetConnectedEntities(DateTime.Today);
            if (connectedExistingZonesServices == null)
                return null;

            return connectedExistingZonesServices.Last();
        }

        private void ProcessData(ImportedZone importedZone, List<ExistingZoneService> existingZoneServices, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName, DateTime pricelistDate)
        {
            if (importedZone.ImportedZoneServiceGroup != null)
                ProcessCountryZoneServices(importedZone.ImportedZoneServiceGroup, existingZoneServices, newAndExistingZones, existingZonesByName);
            else
                CloseNotImportedZoneServices(existingZoneServices, pricelistDate);
        }

        private void CloseNotImportedZoneServices(List<ExistingZoneService> existingZoneServices, DateTime zoneServiceCloseDate)
        {
            if (existingZoneServices != null)
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
        }


        private void ProcessCountryZoneServices(ImportedZoneServiceGroup importedZoneService, List<ExistingZoneService> existingZonesServices, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName)
        {
            ProcessImportedZoneService(importedZoneService, existingZonesServices, newAndExistingZones, existingZonesByName);
        }

        #endregion

        #region Prcessing Not Imported Data Methods

        private void ProcessNotImportedData(IEnumerable<ExistingZone> existingZones, IEnumerable<NotImportedZone> notImportedZones, ExistingZonesServicesByZoneName existingZonesServicesByZoneName)
        {
            CloseServicesForClosedZones(existingZones);
            FillZoneServicesForNotImportedZones(notImportedZones, existingZonesServicesByZoneName);
        }

        private void FillZoneServicesForNotImportedZones(IEnumerable<NotImportedZone> notImportedZones, ExistingZonesServicesByZoneName existingZonesServicesByZoneName)
        {
            if (notImportedZones == null)
                return;

            List<ExistingZoneService> existingZoneServices;
            foreach (NotImportedZone notImportedZone in notImportedZones)
            {
                if (existingZonesServicesByZoneName.TryGetValue(notImportedZone.ZoneName, out existingZoneServices))
                {
                    if (existingZoneServices != null)
                    {
                        NotImportedZoneService notImportedZoneService = this.GetNotImportedZoneService(existingZoneServices, false);
                        if (notImportedZoneService != null)
                            notImportedZone.NotImportedZoneService = notImportedZoneService;
                    }
                }
            }
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

        private void ProcessImportedZoneService(ImportedZoneServiceGroup importedZoneService, List<ExistingZoneService> matchExistingZoneServices, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName)
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


        private void CloseExistingOverlapedZoneServices(ImportedZoneServiceGroup importedZoneService, List<ExistingZoneService> matchExistingServices, out bool shouldNotAddZoneService)
        {
            shouldNotAddZoneService = false;
            foreach (var existingZoneService in matchExistingServices.OrderBy(itm => itm.ZoneServiceEntity.BED))
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
                        if (importedZoneService.EED.HasValue && importedZoneService.EED.VRLessThan(existingZoneService.EED))
                        {
                            existingZoneService.ChangedZoneService = new ChangedZoneService
                            {
                                EntityId = existingZoneService.ZoneServiceEntity.SupplierZoneServiceId,
                                EED = importedZoneService.EED.Value
                            };
                            importedZoneService.ChangedExistingZoneServices.Add(existingZoneService);
                            shouldNotAddZoneService = true;
                            break;
                        }
                    }
                    DateTime existingZoneServiceEed = Utilities.Max(importedZoneService.BED, existingZoneService.BED);
                    existingZoneService.ChangedZoneService = new ChangedZoneService
                    {
                        EntityId = existingZoneService.ZoneServiceEntity.SupplierZoneServiceId,
                        EED = existingZoneServiceEed
                    };
                    importedZoneService.ChangedExistingZoneServices.Add(existingZoneService);
                }
            }
        }


        private void AddImportedZoneService(ImportedZoneServiceGroup importedZoneService, ZonesByName newAndExistingZones, ExistingZonesByName existingZones)
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

        private void AddNewZoneService(ImportedZoneServiceGroup importedZoneService, ref DateTime currentZoneServiceBED, IZone zone, out bool shouldAddMoreZoneServices)
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

        private bool SameZoneServices(ImportedZoneServiceGroup importedZoneService, ExistingZoneService existingZoneService)
        {
            return importedZoneService.BED == existingZoneService.BED
               && sameServiceIds(existingZoneService.ZoneServiceEntity.ReceivedServices, importedZoneService.ServiceIds);
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
