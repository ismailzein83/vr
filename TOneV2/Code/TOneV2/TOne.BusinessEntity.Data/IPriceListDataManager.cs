using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IPriceListDataManager : IDataManager
    {
        List<PriceList> GetPriceList();

        PriceList GetPriceListById(int priceListId);

        bool SavePriceList(PriceList pricelist, out int priceListId);
    }
}
