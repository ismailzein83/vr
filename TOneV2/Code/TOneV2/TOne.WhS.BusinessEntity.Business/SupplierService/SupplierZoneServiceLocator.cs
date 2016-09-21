using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneServiceLocator
    {
        ISupplierZoneServiceReader _reader;
        CarrierAccountManager _carrierAccountManager;

        public SupplierZoneServiceLocator(ISupplierZoneServiceReader reader)
        {
            _reader = reader;
            _carrierAccountManager = new CarrierAccountManager();
        }

        public List<ZoneService> GetSupplierZoneServices(int supplierId, long supplierZoneId)
        {
            SupplierZoneService supplierZoneService;

            if (!HasSupplierZoneServices(supplierId, supplierZoneId, out supplierZoneService))
                return GetDefaultSupplierServices(supplierId);

            return supplierZoneService.EffectiveServices;
        }

        private bool HasSupplierZoneServices(int supplierId, long supplierZoneId, out SupplierZoneService supplierZoneService)
        {
            supplierZoneService = null;

            var supplierZoneServices = _reader.GetSupplierZoneServicesByZone(supplierId);
            if (supplierZoneServices != null && supplierZoneServices.TryGetValue(supplierZoneId, out supplierZoneService))
                return true;

            return false;
        }
        private List<ZoneService> GetDefaultSupplierServices(int supplierId)
        {
            CarrierAccount supplierAccount = _carrierAccountManager.GetCarrierAccount(supplierId);

            if (supplierAccount.SupplierSettings == null)
                throw new NullReferenceException("supplierAccount.SupplierSettings");

            if (supplierAccount.SupplierSettings.DefaultServices == null)
                throw new NullReferenceException("supplierAccount.SupplierSettings.DefaultServices");

            return supplierAccount.SupplierSettings.DefaultServices;
        }
    }
}