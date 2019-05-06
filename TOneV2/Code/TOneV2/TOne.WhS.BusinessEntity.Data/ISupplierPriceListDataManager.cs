using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierPriceListDataManager : IDataManager
    {
        List<SupplierPriceList> GetPriceLists();

        bool ArGetPriceListsUpdated(ref object updateHandle);

    }
}
