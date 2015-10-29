using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface IRatePlanDataManager : IDataManager
    {
        bool InsertSalePriceList(SalePriceList salePriceList, out int salePriceListId);

        bool CloseThenInsertSaleRates(int customerId, List<SaleRate> newSaleRates);
    }
}
