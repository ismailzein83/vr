using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class NewSupplierRateManager
    {
        public void Insert(int priceListId, IEnumerable<NewRate> ratesList)
        {
            INewSupplierRateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<INewSupplierRateDataManager>();
            dataManager.Insert(priceListId, ratesList);
        }
    }
}
