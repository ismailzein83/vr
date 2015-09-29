using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierCodeDataManager:IDataManager
    {
        void InsertSupplierCodes(List<SupplierCode> supplierCodes);
        bool UpdateSupplierCodes(List<long> supplierZoneIds, DateTime effectiveDate);
    }
}
