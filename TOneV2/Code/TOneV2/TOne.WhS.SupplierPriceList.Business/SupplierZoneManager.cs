using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierZoneManager
    {
        public void InsertSupplierZones(List<Zone> supplierZones)
        {
            ISupplierZoneDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            dataManager.InsertSupplierZones(supplierZones);
        }
        public bool UpdateSupplierZones(int supplierId, DateTime effectiveDate)
        {
            ISupplierZoneDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.UpdateSupplierZones(supplierId, effectiveDate);
        }
    }
}
