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
    public class RatePlanServiceReadWithCache : BusinessEntity.Business.ISaleEntityServiceReader
    {
        #region Fields / Constructors

        private BusinessEntity.Business.SaleEntityServiceReadWithCache _reader;

        private SalePriceListOwnerType _ownerType;
        private int _ownerId;

        private BusinessEntity.Entities.SaleEntityDefaultService _defaultService;
        private BusinessEntity.Business.SaleEntityZoneServicesByZone _servicesByZone;

        public RatePlanServiceReadWithCache(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, Changes draft)
        {
            _ownerType = ownerType;
            _ownerId = ownerId;

            _reader = new BusinessEntity.Business.SaleEntityServiceReadWithCache(effectiveOn);

            SetDefaultService(draft != null ? draft.DefaultChanges : null);
            SetServicesByZone(draft != null ? draft.ZoneChanges : null);
        }

        #endregion

        public BusinessEntity.Entities.SaleEntityDefaultService GetSaleEntityDefaultService(SalePriceListOwnerType ownerType, int ownerId)
        {
            return IsSameOwner(ownerType, ownerId) ? _defaultService : _reader.GetSaleEntityDefaultService(ownerType, ownerId);
        }

        public BusinessEntity.Business.SaleEntityZoneServicesByZone GetSaleEntityZoneServicesByZone(SalePriceListOwnerType ownerType, int ownerId)
        {
            return IsSameOwner(ownerType, ownerId) ? _servicesByZone : _reader.GetSaleEntityZoneServicesByZone(ownerType, ownerId);
        }

        #region Private Methods

        private void SetDefaultService(DefaultChanges defaultDraft)
        {
            if (defaultDraft != null)
            {
                if (defaultDraft.ResetService != null)
                {
                    _defaultService = null;
                    return;
                }
                if (defaultDraft.NewService != null)
                {
                    _defaultService = new SaleEntityDefaultService()
                    {
                        Services = defaultDraft.NewService.Services,
                        BED = defaultDraft.NewService.BED,
                        EED = defaultDraft.NewService.EED
                    };
                    return;
                }
            }
            SaleEntityDefaultService existingDefaultService = _reader.GetSaleEntityDefaultService(_ownerType, _ownerId);
            if (existingDefaultService != null)
            {
                _defaultService = new SaleEntityDefaultService()
                {
                    SaleEntityServiceId = existingDefaultService.SaleEntityServiceId,
                    PriceListId = existingDefaultService.PriceListId,
                    Services = existingDefaultService.Services,
                    BED = existingDefaultService.BED,
                    EED = existingDefaultService.EED
                };
            }
            if (defaultDraft != null && defaultDraft.ClosedService != null)
            {
                if (_defaultService == null)
                    throw new NullReferenceException("_defaultService");
                _defaultService.EED = defaultDraft.ClosedService.EED;
            }
        }

        private void SetServicesByZone(IEnumerable<ZoneChanges> zoneDrafts)
        {
            _servicesByZone = new SaleEntityZoneServicesByZone();

            SaleEntityZoneServicesByZone existingServicesByZone = _reader.GetSaleEntityZoneServicesByZone(_ownerType, _ownerId);
            if (existingServicesByZone != null && existingServicesByZone.Values != null)
            {
                foreach (SaleEntityZoneService existingService in existingServicesByZone.Values)
                {
                    var service = new SaleEntityZoneService()
                    {
                        SaleEntityServiceId = existingService.SaleEntityServiceId,
                        PriceListId = existingService.PriceListId,
                        ZoneId = existingService.ZoneId,
                        Services = existingService.Services,
                        BED = existingService.BED,
                        EED = existingService.EED
                    };
                    _servicesByZone.Add(existingService.ZoneId, service);
                }
            }

            if (zoneDrafts != null)
            {
                foreach (ZoneChanges zoneDraft in zoneDrafts)
                {
                    if (zoneDraft.ResetService != null)
                    {
                        _servicesByZone.Remove(zoneDraft.ZoneId);
                        continue;
                    }

                    SaleEntityZoneService service;
                    _servicesByZone.TryGetValue(zoneDraft.ZoneId, out service);

                    if (zoneDraft.NewService != null)
                    {
                        if (service == null)
                        {
                            service = new SaleEntityZoneService()
                            {
                                ZoneId = zoneDraft.ZoneId,
                                Services = zoneDraft.NewService.Services,
                                BED = zoneDraft.NewService.BED,
                                EED = zoneDraft.NewService.EED
                            };
                            _servicesByZone.Add(zoneDraft.ZoneId, service);
                        }
                        else
                        {
                            service.SaleEntityServiceId = 0;
                            service.PriceListId = 0;
                            service.Services = zoneDraft.NewService.Services;
                            service.BED = zoneDraft.NewService.BED;
                            service.EED = zoneDraft.NewService.EED;
                        }
                    }
                    else if (zoneDraft.ClosedService != null)
                    {
                        if (service == null)
                            throw new NullReferenceException("service");
                        service.EED = zoneDraft.ClosedService.EED;
                    }
                }
            }
        }

        private bool IsSameOwner(SalePriceListOwnerType ownerType, int ownerId)
        {
            return (ownerType == _ownerType && ownerId == _ownerId);
        }

        #endregion
    }
}
