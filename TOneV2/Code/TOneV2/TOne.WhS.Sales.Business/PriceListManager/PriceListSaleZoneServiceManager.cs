using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.RateManagement.Service.Zone;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class PriceListSaleZoneServiceManager
    {
        public void ProcessSaleZoneServices(IProcessSaleZoneServicesContext context)
        {
            Process(context.SaleZoneServicesToAdd, context.SaleZoneServicesToClose, context.ExistingSaleZoneServices, context.ExistingZones);
            context.NewSaleZoneServices = context.SaleZoneServicesToAdd.SelectMany(x => x.NewSaleZoneServices);
            context.ChangedSaleZoneServices = context.ExistingSaleZoneServices.Where(x => x.ChangedSaleZoneService != null).Select(x => x.ChangedSaleZoneService);
        }
        public void Process(IEnumerable<SaleZoneServiceToAdd> saleZoneServicesToAdd, IEnumerable<SaleZoneServiceToClose> saleZoneServicesToClose, IEnumerable<ExistingSaleZoneService> existingSaleZoneServices, IEnumerable<ExistingZone> existingZones)
        {
            ExistingSaleZoneServicesByZoneName existingSaleZoneServicesByZoneName = StructureExistingSaleZoneServicesByZoneName(existingSaleZoneServices);
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);

            foreach (SaleZoneServiceToAdd saleZoneServiceToAdd in saleZoneServicesToAdd)
            {
                List<ExistingSaleZoneService> matchedExistingSaleZoneServices;

                if (existingSaleZoneServicesByZoneName.TryGetValue(saleZoneServiceToAdd.ZoneName, out matchedExistingSaleZoneServices))
                    CloseOverlappedExistingSaleZoneServices(saleZoneServiceToAdd, matchedExistingSaleZoneServices);

                ProcessSaleZoneServiceToAdd(saleZoneServiceToAdd, existingZonesByName);
            }

            foreach (SaleZoneServiceToClose saleZoneServiceToClose in saleZoneServicesToClose)
            {
                List<ExistingSaleZoneService> matchedExistingSaleZoneServices;
                if (existingSaleZoneServicesByZoneName.TryGetValue(saleZoneServiceToClose.ZoneName, out matchedExistingSaleZoneServices))
                    CloseExistingSaleZoneServices(saleZoneServiceToClose, matchedExistingSaleZoneServices);
            }
        }

        private ExistingSaleZoneServicesByZoneName StructureExistingSaleZoneServicesByZoneName(IEnumerable<ExistingSaleZoneService> existingSaleZoneServices)
        {
            var servicesByZoneName = new ExistingSaleZoneServicesByZoneName();

            if (existingSaleZoneServices == null)
                return servicesByZoneName;

            List<ExistingSaleZoneService> serviceList = null;
            var saleZoneManager = new SaleZoneManager();

            foreach (ExistingSaleZoneService existingService in existingSaleZoneServices)
            {
                string zoneName = saleZoneManager.GetSaleZoneName(existingService.SaleEntityZoneServiceEntity.ZoneId);

                if (!servicesByZoneName.TryGetValue(zoneName, out serviceList))
                {
                    serviceList = new List<ExistingSaleZoneService>();
                    servicesByZoneName.Add(zoneName, serviceList);
                }

                serviceList.Add(existingService);
            }

            return servicesByZoneName;
        }
        private ExistingZonesByName StructureExistingZonesByName(IEnumerable<ExistingZone> existingZones)
        {
            ExistingZonesByName existingZonesByName = new ExistingZonesByName();
            List<ExistingZone> existingZoneList = null;

            foreach (ExistingZone item in existingZones)
            {
                if (!existingZonesByName.TryGetValue(item.Name, out existingZoneList))
                {
                    existingZoneList = new List<ExistingZone>();
                    existingZonesByName.Add(item.Name, existingZoneList);
                }

                existingZoneList.Add(item);
            }

            return existingZonesByName;
        }

        private void CloseOverlappedExistingSaleZoneServices(SaleZoneServiceToAdd saleZoneServiceToAdd, IEnumerable<ExistingSaleZoneService> matchedExistingSaleZoneServices)
        {
            foreach (ExistingSaleZoneService existingSaleZoneService in matchedExistingSaleZoneServices)
            {
                if (existingSaleZoneService.IsOverlappedWith(saleZoneServiceToAdd))
                {
                    //if (AreSameServices(saleZoneServiceToAdd, existingSaleZoneService))
                    //{

                    //}
                    existingSaleZoneService.ChangedSaleZoneService = new ChangedSaleZoneService()
                    {
                        SaleEntityServiceId = existingSaleZoneService.SaleEntityZoneServiceEntity.SaleEntityServiceId,
                        EED = Utilities.Max(existingSaleZoneService.BED, saleZoneServiceToAdd.BED)
                    };
                    saleZoneServiceToAdd.ChangedExistingSaleZoneServices.Add(existingSaleZoneService);
                }
            }
        }
        private void ProcessSaleZoneServiceToAdd(SaleZoneServiceToAdd saleZoneServiceToAdd, ExistingZonesByName existingZonesByName)
        {
            List<ExistingZone> matchedExistingZones;
            existingZonesByName.TryGetValue(saleZoneServiceToAdd.ZoneName, out matchedExistingZones);

            DateTime newSaleZoneServiceBED = saleZoneServiceToAdd.BED;
            bool shouldAddNewSaleZoneServices = true;

            foreach (var existingZone in matchedExistingZones.OrderBy(x => x.BED))
            {
                if (existingZone.EED.VRGreaterThan(existingZone.BED) && existingZone.EED.VRGreaterThan(newSaleZoneServiceBED) && saleZoneServiceToAdd.EED.VRGreaterThan(existingZone.BED))
                {
                    AddNewSaleZoneService(saleZoneServiceToAdd, ref newSaleZoneServiceBED, existingZone, out shouldAddNewSaleZoneServices);
                    if (!shouldAddNewSaleZoneServices)
                        break;
                }
            }
        }
        private void AddNewSaleZoneService(SaleZoneServiceToAdd SaleZoneServiceToAdd, ref DateTime newSaleZoneServiceBED, ExistingZone existingZone, out bool shouldAddNewSaleZoneServices)
        {
            shouldAddNewSaleZoneServices = false;

            var newSaleZoneService = new NewSaleZoneService
            {
                Services = SaleZoneServiceToAdd.Services,
                SaleZoneId = SaleZoneServiceToAdd.ZoneId,
                BED = Utilities.Max(existingZone.BED, newSaleZoneServiceBED),
                EED = SaleZoneServiceToAdd.EED
            };

            if (newSaleZoneService.EED.VRGreaterThan(existingZone.EED)) // => existingZone.EED != null
            {
                newSaleZoneService.EED = existingZone.EED;
                newSaleZoneServiceBED = newSaleZoneService.EED.Value;
                shouldAddNewSaleZoneServices = true;
            }

            SaleZoneServiceToAdd.NewSaleZoneServices.Add(newSaleZoneService);
        }

        private void CloseExistingSaleZoneServices(SaleZoneServiceToClose saleZoneServiceToClose, IEnumerable<ExistingSaleZoneService> matchedExistingSaleZoneServices)
        {
            foreach (ExistingSaleZoneService existingSaleZoneServices in matchedExistingSaleZoneServices)
            {
                if (existingSaleZoneServices.EED.VRGreaterThan(saleZoneServiceToClose.CloseEffectiveDate))
                {
                    existingSaleZoneServices.ChangedSaleZoneService = new ChangedSaleZoneService()
                    {
                        SaleEntityServiceId = existingSaleZoneServices.SaleEntityZoneServiceEntity.SaleEntityServiceId,
                        EED = Utilities.Max(existingSaleZoneServices.BED, saleZoneServiceToClose.CloseEffectiveDate)
                    };
                    saleZoneServiceToClose.ChangedExistingSaleZoneServices.Add(existingSaleZoneServices);
                }
            }
        }

        private bool AreSameServices(SaleZoneServiceToAdd saleZoneServiceToAdd, ExistingSaleZoneService existingSaleZoneService)
        {
            if (saleZoneServiceToAdd.BED == existingSaleZoneService.BED)
            {
                List<ZoneService> newServices = saleZoneServiceToAdd.Services;
                List<ZoneService> existingServices = existingSaleZoneService.SaleEntityZoneServiceEntity.Services;
                if (newServices.Count == existingServices.Count)
                {
                    foreach (ZoneService newService in newServices)
                    {
                        if (!existingServices.Any(x => x.ServiceId == newService.ServiceId))
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
