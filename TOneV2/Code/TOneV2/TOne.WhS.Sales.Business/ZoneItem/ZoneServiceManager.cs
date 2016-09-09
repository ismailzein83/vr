using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class ZoneServiceManager
    {
        private SaleEntityServiceLocator _serviceLocator;

        public ZoneServiceManager(DateTime effectiveOn)
        {
            _serviceLocator = new SaleEntityServiceLocator(new SaleEntityServiceReadWithCache(effectiveOn));
        }

        public void SetSellingProductZoneService(ZoneItem zoneItem, int sellingProductId, ZoneChanges zoneDraft, DraftNewDefaultService newDefaultService)
        {
            SaleEntityService currentService = _serviceLocator.GetSellingProductZoneService(sellingProductId, zoneItem.ZoneId);

            if (currentService != null)
            {
                zoneItem.CurrentServices = currentService.Services;
                zoneItem.CurrentServiceBED = currentService.BED;
                zoneItem.CurrentServiceEED = currentService.EED;
                zoneItem.IsCurrentServiceEditable = currentService.Source == SaleEntityServiceSource.ProductZone;
            }

            SetDraftZoneService(zoneItem, zoneDraft);
            SetSellingProductEffectiveZoneService(zoneItem, sellingProductId, newDefaultService);
        }

        public void SetCustomerZoneService(ZoneItem zoneItem, int customerId, int sellingProductId, ZoneChanges zoneDraft, DraftNewDefaultService newDefaultService)
        {
            SaleEntityService currentService = _serviceLocator.GetCustomerZoneService(customerId, sellingProductId, zoneItem.ZoneId);

            if (currentService != null)
            {
                zoneItem.CurrentServices = currentService.Services;
                zoneItem.CurrentServiceBED = currentService.BED;
                zoneItem.CurrentServiceEED = currentService.EED;
                zoneItem.IsCurrentServiceEditable = currentService.Source == SaleEntityServiceSource.CustomerZone;
            }

            SetDraftZoneService(zoneItem, zoneDraft);
            SetCustomerEffectiveZoneService(zoneItem, customerId, sellingProductId, newDefaultService);
        }

        private void SetDraftZoneService(ZoneItem zoneItem, ZoneChanges zoneDraft)
        {
            if (zoneDraft != null)
            {
                zoneItem.NewService = zoneDraft.NewService;
                zoneItem.ClosedService = zoneDraft.ClosedService;
                zoneItem.ResetService = zoneDraft.ResetService;
            }
        }

        private void SetSellingProductEffectiveZoneService(ZoneItem zoneItem, int sellingProductId, DraftNewDefaultService newDefaultService)
        {
            SaleEntityService defaultService;

            if (zoneItem.NewService != null)
            {
                zoneItem.EffectiveServices = zoneItem.NewService.Services;
                return;
            }
            if (zoneItem.ClosedService != null)
            {
                zoneItem.EffectiveServices = zoneItem.CurrentServices;
                return;
            }
            if (zoneItem.ResetService != null)
            {
                if (newDefaultService != null)
                    zoneItem.EffectiveServices = newDefaultService.Services;
                else
                {
                    defaultService = _serviceLocator.GetSellingProductDefaultService(sellingProductId);
                    if (defaultService != null)
                        zoneItem.EffectiveServices = defaultService.Services;
                }
                return;
            }

            if (zoneItem.IsCurrentServiceEditable.HasValue && zoneItem.IsCurrentServiceEditable.Value)
            {
                zoneItem.EffectiveServices = zoneItem.CurrentServices;
                return;
            }

            if (newDefaultService != null)
                zoneItem.EffectiveServices = newDefaultService.Services;
            else
            {
                defaultService = _serviceLocator.GetSellingProductDefaultService(sellingProductId);
                if (defaultService != null)
                    zoneItem.EffectiveServices = defaultService.Services;
            }
        }

        private void SetCustomerEffectiveZoneService(ZoneItem zoneItem, int customerId, int sellingProductId, DraftNewDefaultService newDefaultService)
        {
            SaleEntityService inheritedService;

            if (zoneItem.NewService != null)
            {
                zoneItem.EffectiveServices = zoneItem.NewService.Services;
                return;
            }
            if (zoneItem.ClosedService != null)
            {
                zoneItem.EffectiveServices = zoneItem.CurrentServices;
                return;
            }
            if (zoneItem.ResetService != null)
            {
                if (newDefaultService != null)
                    zoneItem.EffectiveServices = newDefaultService.Services;
                else
                {
                    inheritedService = _serviceLocator.GetCustomerInheritedZoneService(customerId, sellingProductId, zoneItem.ZoneId);
                    if (inheritedService != null)
                        zoneItem.EffectiveServices = inheritedService.Services;
                }
                return;
            }

            if (zoneItem.IsCurrentServiceEditable.HasValue && zoneItem.IsCurrentServiceEditable.Value)
            {
                zoneItem.EffectiveServices = zoneItem.CurrentServices;
                return;
            }
            
            if (newDefaultService != null)
                zoneItem.EffectiveServices = newDefaultService.Services;
            else
            {
                inheritedService = _serviceLocator.GetCustomerInheritedZoneService(customerId, sellingProductId, zoneItem.ZoneId);
                if (inheritedService != null)
                    zoneItem.EffectiveServices = inheritedService.Services;
            }
        }
    }
}
