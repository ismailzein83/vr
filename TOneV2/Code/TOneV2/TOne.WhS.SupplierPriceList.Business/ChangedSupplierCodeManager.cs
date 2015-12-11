using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ChangedSupplierCodeManager
    {
        public void Insert(int priceListId, IEnumerable<Entities.SPL.ChangedCode> changedCodes)
        {
            IChangedSupplierCodeDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IChangedSupplierCodeDataManager>();
            dataManager.Insert(priceListId, changedCodes);
        }
    }
}
