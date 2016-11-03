using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
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

        public SupplierEntityService GetSupplierZoneServices(int supplierId, long supplierZoneId)
        {
            SupplierZoneService supplierZoneService;
            Dictionary<int, ZoneService> servicesAndChildServices = new Dictionary<int, ZoneService>();

            if (HasSupplierZoneServices(supplierId, supplierZoneId, out supplierZoneService))
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
                SupplierDefaultService defaultService = GetDefaultSupplierServices(supplierId);
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
        
        private bool HasSupplierZoneServices(int supplierId, long supplierZoneId, out SupplierZoneService supplierZoneService)
        {
            supplierZoneService = null;

            var supplierZoneServices = _reader.GetSupplierZoneServicesByZone(supplierId);
            if (supplierZoneServices != null && supplierZoneServices.TryGetValue(supplierZoneId, out supplierZoneService))
                return true;

            return false;
        }

        private SupplierDefaultService GetDefaultSupplierServices(int supplierId)
        {
            var defaultService = _reader.GetSupplierDefaultService(supplierId);
            if (defaultService == null)
                throw new DataIntegrityValidationException(string.Format("No default services set for Supplier with id {0}", supplierId));

            return defaultService;

        }

        #endregion
    }
}