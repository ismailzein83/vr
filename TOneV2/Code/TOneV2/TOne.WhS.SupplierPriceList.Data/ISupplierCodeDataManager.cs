using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierCodeDataManager : IDataManager
    {
        void InsertSupplierCodes(List<Zone> supplierZones);
    }
}
