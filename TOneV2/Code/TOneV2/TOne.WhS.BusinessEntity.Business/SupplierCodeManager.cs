using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierCodeManager
    {
        public void InsertSupplierCodes(List<SupplierCode> supplierCodes)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            dataManager.InsertSupplierCodes(supplierCodes);
        }
        public bool UpdateSupplierCodes(List<long> supplierZoneIds, DateTime effectiveDate)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.UpdateSupplierCodes(supplierZoneIds, effectiveDate);
        }

        public List<SupplierCode> GetSupplierCodes(DateTime minimumDate)
        {
            ISupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.GetSupplierCodes(minimumDate);
        }

    }
}
