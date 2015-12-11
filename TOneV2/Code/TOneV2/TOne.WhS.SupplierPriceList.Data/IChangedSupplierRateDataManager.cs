using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface IChangedSupplierRateDataManager : IDataManager
    {
        void Insert(int priceListId, IEnumerable<ChangedRate> changedRates);
    }
}
