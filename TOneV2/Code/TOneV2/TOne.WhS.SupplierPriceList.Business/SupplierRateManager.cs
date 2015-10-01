using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;


namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierRateManager
    {
        public void InsertSupplierRates(List<Zone> supplierRates)
        {
            ISupplierRateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            dataManager.InsertSupplierRates(supplierRates);
        }
    }
}
