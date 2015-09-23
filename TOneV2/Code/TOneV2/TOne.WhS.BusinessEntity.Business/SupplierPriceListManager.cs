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
        public bool AddSupplierPriceList(int supplierAccountId)
        {
            ISupplierPriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
           return dataManager.AddSupplierPriceList(supplierAccountId);
        }
    }
}
