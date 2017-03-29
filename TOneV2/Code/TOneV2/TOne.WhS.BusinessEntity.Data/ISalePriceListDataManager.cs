using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISalePriceListDataManager : IDataManager
    {
        List<SalePriceList> GetPriceLists();
        bool Update(SalePriceList salePriceList);

        bool Insert(SalePriceList salePriceList);
        bool ArGetSalePriceListsUpdated(ref object updateHandle);
        bool UpdatePriceListNotification(IEnumerable<int> customerIds, int ownerId, long processInstanceId);
        void SavePriceListsToDb(List<SalePriceList> salePriceLists);

    }
}
