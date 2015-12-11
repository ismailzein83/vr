using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class NewSupplierCodeManager
    {
        public void Insert(int priceListId, IEnumerable<NewCode> codesList)
        {
            INewSupplierCodeDataManager dataManager = SupPLDataManagerFactory.GetDataManager<INewSupplierCodeDataManager>();
            dataManager.Insert(priceListId, codesList);
        }
    }
}
