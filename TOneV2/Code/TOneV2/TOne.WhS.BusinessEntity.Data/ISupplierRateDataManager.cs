using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierRateDataManager:IDataManager
    {
        void InsertSupplierRates(List<SupplierRate> supplierRates);
        bool UpdateSupplierRates(List<long> zoneIds, DateTime effectiveDate);
        List<SupplierRate> GetSupplierRates(DateTime minimumDate);
    }
}
