using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierPriceListManager
    {
        public bool AddSupplierPriceList(int supplierAccountId, out int supplierPriceListId)
        {
            ISupplierPriceListDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            return dataManager.AddSupplierPriceList(supplierAccountId, out supplierPriceListId);
        }
    }
}
