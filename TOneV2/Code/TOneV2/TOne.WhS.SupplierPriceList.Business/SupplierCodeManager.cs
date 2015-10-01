using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierCodeManager
    {
        public void InsertSupplierCodes(List<Code> supplierCodes)
        {
            ISupplierCodeDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            dataManager.InsertSupplierCodes(supplierCodes);
        }
        public bool UpdateSupplierCodes(List<long> supplierZoneIds, DateTime effectiveDate)
        {
            ISupplierCodeDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
            return dataManager.UpdateSupplierCodes(supplierZoneIds, effectiveDate);
        }
    }
}
