using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneServiceLocator
    {
        #region Local Variables
        ISupplierZoneServiceReader _reader;
        CarrierAccountManager _carrierAccountManager;
        #endregion


        #region Public Methods
        public SupplierZoneServiceLocator(ISupplierZoneServiceReader reader)
        {
            _reader = reader;
            _carrierAccountManager = new CarrierAccountManager();
        }

        public SupplierEntityService GetSupplierZoneServices(int supplierId, long supplierZoneId, DateTime? effectiveOn)
        {
            SupplierZoneService supplierZoneService;
            Dictionary<int, ZoneService> servicesAndChildServices = new Dictionary<int, ZoneService>();

            if (HasSupplierZoneServices(supplierId, supplierZoneId, effectiveOn, out supplierZoneService))
            {
                return new SupplierEntityService()
                {
                    Source = SupplierEntityServiceSource.SupplierZone,
                    Services = supplierZoneService.EffectiveServices,
                    BED = supplierZoneService.BED,
                    EED = supplierZoneService.EED
                };
            }
            else
            {
                SupplierDefaultService defaultService = GetDefaultSupplierServices(supplierId, effectiveOn);
                return new SupplierEntityService()
                {
                    Source = SupplierEntityServiceSource.Supplier,
                    Services = defaultService.EffectiveServices,
                    BED = defaultService.BED,
                    EED = defaultService.EED
                };
            }
        }
        #endregion


        #region Private Methods

        private bool HasSupplierZoneServices(int supplierId, long supplierZoneId, DateTime? effectiveOn, out SupplierZoneService supplierZoneService)
        {
            supplierZoneService = _reader.GetSupplierZoneServicesByZone(supplierId, supplierZoneId, effectiveOn);
            return supplierZoneService != null;
        }

        private SupplierDefaultService GetDefaultSupplierServices(int supplierId, DateTime? effectiveOn)
        {
            var defaultService = _reader.GetSupplierDefaultService(supplierId, effectiveOn);
            if (defaultService == null)
                throw new DataIntegrityValidationException(string.Format("No default services set for Supplier with id {0}", supplierId));

            return defaultService;

        }

        #endregion
    }
}