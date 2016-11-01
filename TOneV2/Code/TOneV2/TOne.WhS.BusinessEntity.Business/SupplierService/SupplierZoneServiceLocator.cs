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
                List<ZoneService> defaultServices = GetDefaultSupplierServices(supplierId);
                //SupplierDefaultService defaultService = GetDefaultSupplierServices(supplierId);
                return new SupplierEntityService()
                {
                    Source = SupplierEntityServiceSource.Supplier,
                    Services = defaultServices,
                    //BED = defaultService.BED,
                    //EED = defaultService.EED
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

        private SupplierDefaultService GetDefaultSupplierServicesNew(int supplierId)
        {
            var defaultService = _reader.GetSupplierDefaultService(supplierId);
            if (defaultService == null)
                throw new DataIntegrityValidationException(string.Format("No default services set for Supplier with id {0}", supplierId));

            return defaultService;

        }

        private List<ZoneService> GetDefaultSupplierServices(int supplierId)
        {
            //TODO: MJA remove this method and use the new one after renaming it
            CarrierAccount supplierAccount = _carrierAccountManager.GetCarrierAccount(supplierId);

            if (supplierAccount.SupplierSettings == null)
                throw new NullReferenceException(string.Format("supplierAccount.SupplierSettings for Supplier Id: {0}", supplierId));

            if (supplierAccount.SupplierSettings.DefaultServices == null)
                throw new NullReferenceException(string.Format("supplierAccount.SupplierSettings.DefaultServices for Supplier Id: {0}", supplierId));

            if (supplierAccount.SupplierSettings.DefaultServices.Count == 0)
                throw new Exception(string.Format("supplierAccount.SupplierSettings.DefaultServices for Supplier Id: {0} count is 0.", supplierId));

            return supplierAccount.SupplierSettings.DefaultServices;
        }

        #endregion
    }
}