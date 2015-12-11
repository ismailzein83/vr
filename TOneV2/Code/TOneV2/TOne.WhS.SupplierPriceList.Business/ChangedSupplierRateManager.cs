using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ChangedSupplierRateManager
    {
        public void Insert(int priceListId, IEnumerable<Entities.SPL.ChangedRate> changedRates)
        {
            IChangedSupplierRateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IChangedSupplierRateDataManager>();
            dataManager.Insert(priceListId, changedRates);
        }
    }
}
