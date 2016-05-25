using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface IRateDataManager : IDataManager
    {
        bool CloseRates(IEnumerable<DraftChangedRate> rateChanges);
        bool InsertRates(IEnumerable<DraftNewRate> newRates, int priceListId);
    }
}
