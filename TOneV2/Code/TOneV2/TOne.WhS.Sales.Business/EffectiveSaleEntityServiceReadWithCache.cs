using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class EffectiveSaleEntityServiceReadWithCache : BusinessEntity.Business.ISaleEntityServiceReader
    {
        #region Fields / Constructors

        private BusinessEntity.Business.SaleEntityServiceReadWithCache _reader;

        private SalePriceListOwnerType _ownerType;
        private int _ownerId;

        private DefaultChanges _defaultDraft;
        private IEnumerable<ZoneChanges> _zoneDrafts;

        public EffectiveSaleEntityServiceReadWithCache(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, Changes draft)
        {
            _ownerType = ownerType;
            _ownerId = ownerId;

            _reader = new BusinessEntity.Business.SaleEntityServiceReadWithCache(effectiveOn);

            if (draft != null)
            {
                if (draft.DefaultChanges != null)
                    _defaultDraft = draft.DefaultChanges;

                if (draft.ZoneChanges != null)
                    _zoneDrafts = draft.ZoneChanges;
            }
        }

        #endregion

        public BusinessEntity.Entities.SaleEntityDefaultService GetSaleEntityDefaultService(SalePriceListOwnerType ownerType, int ownerId)
        {
            SaleEntityDefaultService defaultService = null;

            if (IsSameOwner(ownerType, ownerId) && _defaultDraft != null)
            {
                if (_defaultDraft.ResetService != null) // Return null and let the service locator work its way up the inheritance chain
                    return defaultService;

                if (_defaultDraft.NewService != null)
                {
                    defaultService = new SaleEntityDefaultService()
                    {
                        Services = _defaultDraft.NewService.Services,
                        BED = _defaultDraft.NewService.BED,
                        EED = _defaultDraft.NewService.EED
                    };
                    return defaultService;
                }
            }

            SaleEntityDefaultService existingDefaultService = _reader.GetSaleEntityDefaultService(ownerType, ownerId);
            if (existingDefaultService != null)
            {
                defaultService = new SaleEntityDefaultService()
                {
                    Services = existingDefaultService.Services,
                    BED = existingDefaultService.BED,
                    EED = existingDefaultService.EED
                };
            }

            if (IsSameOwner(ownerType, ownerId) && _defaultDraft != null && _defaultDraft.ClosedService != null)
            {
                if (defaultService == null) // Only existing services can be closed
                    throw new NullReferenceException("currentDefaultService");
                defaultService.EED = _defaultDraft.ClosedService.EED;
            }

            return defaultService;
        }

        public BusinessEntity.Business.SaleEntityZoneServicesByZone GetSaleEntityZoneServicesByZone(SalePriceListOwnerType ownerType, int ownerId)
        {
            var servicesByZone = new SaleEntityZoneServicesByZone();

            SaleEntityZoneServicesByZone existingServicesByZone = _reader.GetSaleEntityZoneServicesByZone(ownerType, ownerId);
            if (existingServicesByZone != null)
            {
                foreach (KeyValuePair<long, SaleEntityZoneService> kvp in existingServicesByZone)
                {
                    if (kvp.Value == null)
                        continue; // Is this an implicit action? Should an exception be thrown here instead?
                    SaleEntityZoneService service = new SaleEntityZoneService()
                    {
                        SaleEntityServiceId = kvp.Value.SaleEntityServiceId,
                        PriceListId = kvp.Value.PriceListId,
                        Services = kvp.Value.Services,
                        BED = kvp.Value.BED,
                        EED = kvp.Value.EED
                    };
                    servicesByZone.Add(kvp.Key, service);
                }
            }

            if (IsSameOwner(ownerType, ownerId) && _zoneDrafts != null)
            {
                foreach (ZoneChanges zoneDraft in _zoneDrafts)
                {
                    if (zoneDraft.ResetService != null)
                        servicesByZone.Remove(zoneDraft.ZoneId);

                    SaleEntityZoneService service;
                    servicesByZone.TryGetValue(zoneDraft.ZoneId, out service);

                    if (zoneDraft.NewService != null)
                    {
                        if (service == null)
                        {
                            service = new SaleEntityZoneService();
                            servicesByZone.Add(zoneDraft.ZoneId, service);
                        }
                        else
                        {
                            service.SaleEntityServiceId = 0;
                            service.PriceListId = 0;
                        }
                        service.Services = zoneDraft.NewService.Services;
                        service.BED = zoneDraft.NewService.BED;
                        service.EED = zoneDraft.NewService.EED;
                    }
                    else if (zoneDraft.ClosedService != null)
                    {
                        if (service == null) // Only existing services can be closed
                            throw new NullReferenceException("service");
                        service.EED = zoneDraft.ClosedService.EED;
                    }
                }
            }

            return servicesByZone;
        }

        #region Private Methods

        private bool IsSameOwner(SalePriceListOwnerType ownerType, int ownerId)
        {
            return (ownerType == _ownerType && ownerId == _ownerId);
        }

        #endregion
    }
}
