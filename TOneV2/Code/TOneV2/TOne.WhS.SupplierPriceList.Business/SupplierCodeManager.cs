using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierCodeManager
    {
        public void InsertSupplierCodes(List<Zone> supplierCodes)
        {
            ISupplierCodeDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            dataManager.InsertSupplierCodes(supplierCodes);
        }
    }
}
