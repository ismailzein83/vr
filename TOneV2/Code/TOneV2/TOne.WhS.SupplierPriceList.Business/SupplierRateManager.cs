using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierRateManager
    {
        public void InsertSupplierRates(List<Rate> supplierRates)
        {
            ISupplierRateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            dataManager.InsertSupplierRates(supplierRates);
        }
        public bool UpdateSupplierRates(List<long> zoneIds, DateTime effectiveDate)
        {
            ISupplierRateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.UpdateSupplierRates(zoneIds, effectiveDate);
        }
    }
}
