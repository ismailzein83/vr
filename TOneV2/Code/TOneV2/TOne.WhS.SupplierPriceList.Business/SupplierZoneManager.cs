using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
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

    }
}
