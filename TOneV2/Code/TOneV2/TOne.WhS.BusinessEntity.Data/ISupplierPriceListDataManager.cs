using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierPriceListDataManager : IDataManager
    {
        SupplierPriceList GetPriceList(int priceListId);

        List<SupplierPriceList> GetPriceLists();

        bool ArGetPriceListsUpdated(ref object updateHandle);

    }
}
