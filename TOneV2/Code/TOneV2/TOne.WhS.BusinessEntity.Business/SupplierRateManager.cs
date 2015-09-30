using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateManager
    {
        public void InsertSupplierRates(List<SupplierRate> supplierRates)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            dataManager.InsertSupplierRates(supplierRates);
        }
        public bool UpdateSupplierRates(List<long> zoneIds, DateTime effectiveDate)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.UpdateSupplierRates(zoneIds, effectiveDate);
        }
        public List<SupplierRate> GetSupplierRates(DateTime minimumDate)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetSupplierRates(minimumDate);
        }
    }
}
