using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleZonePackageDataManager:IDataManager
    {
         List<SaleZonePackage> GetSaleZonePackages();

         bool AreZonePackagesUpdated(ref object updateHandle);
    }
}
