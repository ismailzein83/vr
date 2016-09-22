using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneServiceLocator
    {
        #region Local Variables
        ISupplierZoneServiceReader _reader;
        CarrierAccountManager _carrierAccountManager;
        ZoneServiceConfigManager _zoneServiceConfigManager;
        #endregion


        #region Public Methods
        public SupplierZoneServiceLocator(ISupplierZoneServiceReader reader)
        {
            _reader = reader;
            _carrierAccountManager = new CarrierAccountManager();
            _zoneServiceConfigManager = new ZoneServiceConfigManager();
        }

        public List<ZoneService> GetRoutingSupplierZoneServices(int supplierId, long supplierZoneId, bool withChildServices = false)
        {
            SupplierZoneService supplierZoneService;
            Dictionary<int, ZoneService> servicesAndChildServices = new Dictionary<int, ZoneService>();

            if (HasSupplierZoneServices(supplierId, supplierZoneId, out supplierZoneService))
            {
                // Supplier Zone has Services
                GetAllRelatedZoneServices(supplierZoneService.EffectiveServices, servicesAndChildServices, withChildServices);
                return servicesAndChildServices.Values.ToList();
            }
            else
            {
                //Get default supplier services
                GetAllRelatedZoneServices(GetDefaultSupplierServices(supplierId), servicesAndChildServices, withChildServices);
                return servicesAndChildServices.Values.ToList();
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
        private List<ZoneService> GetDefaultSupplierServices(int supplierId)
        {
            CarrierAccount supplierAccount = _carrierAccountManager.GetCarrierAccount(supplierId);

            if (supplierAccount.SupplierSettings == null)
                throw new NullReferenceException(string.Format("supplierAccount.SupplierSettings for Supplier Id: {0}", supplierId));

            if (supplierAccount.SupplierSettings.DefaultServices == null)
                throw new NullReferenceException(string.Format("supplierAccount.SupplierSettings.DefaultServices for Supplier Id: {0}", supplierId));

            if (supplierAccount.SupplierSettings.DefaultServices.Count == 0)
                throw new Exception(string.Format("supplierAccount.SupplierSettings.DefaultServices for Supplier Id: {0} count is 0.", supplierId));

            return supplierAccount.SupplierSettings.DefaultServices;
        }

        private void GetAllRelatedZoneServices(List<ZoneService> zoneServices, Dictionary<int, ZoneService> allZoneServices, bool withChildServices)
        {
            if (zoneServices == null || zoneServices.Count == 0)
                return;

            CheckIfDuplicateBeforeAdd(zoneServices, allZoneServices);

            if (withChildServices)
            {
                List<ZoneService> childZoneServices = _zoneServiceConfigManager.GetChildServicesByZoneServices(zoneServices);
                GetAllRelatedZoneServices(childZoneServices, allZoneServices, withChildServices);
            }
        }
        private void CheckIfDuplicateBeforeAdd(List<ZoneService> zoneServices, Dictionary<int, ZoneService> allZoneServices)
        {
            foreach (ZoneService zoneService in zoneServices)
            {
                ZoneService duplicateZoneServices = allZoneServices.GetRecord(zoneService.ServiceId);
                if (duplicateZoneServices == null)
                {
                    allZoneServices.Add(zoneService.ServiceId, zoneService);
                }
            }
        }
        #endregion
    }
}