using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
   public class ImportPriceListManager
    {
       public void InsertPriceListObject(List<Zone> supplierZones,List<Code> codesToBeDeleted, int supplierId, int priceListId)
       {
           IImportPriceListDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IImportPriceListDataManager>();
           dataManager.InsertPriceListObject(supplierZones, codesToBeDeleted, supplierId,priceListId);
       }
    }

}
