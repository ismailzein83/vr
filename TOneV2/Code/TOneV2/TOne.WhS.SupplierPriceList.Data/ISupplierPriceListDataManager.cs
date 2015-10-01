using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierPriceListDataManager : IDataManager
    {
        bool AddSupplierPriceList(int supplierAccountId, out int supplierPriceListId);
    }
}
