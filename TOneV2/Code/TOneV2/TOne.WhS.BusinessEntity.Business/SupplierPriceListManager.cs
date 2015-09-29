using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierPriceListManager
    {
        public bool AddSupplierPriceList(int supplierAccountId, out int supplierPriceListId)
        {
            ISupplierPriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            return dataManager.AddSupplierPriceList(supplierAccountId, out supplierPriceListId);
        }
    }
}
