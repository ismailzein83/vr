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

        public SupplierZoneServiceLocator(ISupplierZoneServiceReader reader)
        {
            _reader = reader;
        }

        public SupplierZoneService GetSupplierZoneService(int supplierId, long supplierZoneId)
        {
            var supplierZoneServices = _reader.GetSupplierZoneServicesByZone(supplierId);
            if (supplierZoneServices != null)
            {
                SupplierZoneService supplierZoneService;
                if (supplierZoneServices.TryGetValue(supplierZoneId, out supplierZoneService))
                {
                    return supplierZoneService;
                }
            } 
            return null;
        }
    }
}
