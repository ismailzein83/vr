using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierCodeDataManager : IDataManager
    {
        void InsertSupplierCodes(List<Code> supplierCodes);
        bool UpdateSupplierCodes(List<long> supplierZoneIds, DateTime effectiveDate);
    }
}
