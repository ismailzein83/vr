using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneManager
    {
       
        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.GetSupplierZones(supplierId, effectiveDate);
        }
      
        public int ReserveIDRange(int numberOfIDs)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.ReserveIDRange(numberOfIDs);
        }

    }
}
