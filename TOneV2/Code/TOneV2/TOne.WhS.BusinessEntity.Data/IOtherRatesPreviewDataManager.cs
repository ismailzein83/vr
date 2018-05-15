using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IOtherRatesPreviewDataManager : IDataManager
    {
        IEnumerable<SalePricelistRateChange> GetFilteredRatesPreviewByProcessInstanceId(int processInstanceId,string zoneName,int customerId);
        IEnumerable<SalePricelistRateChange> GetFilteredRatesPreviewByPriceListId(int pricelistId, string zoneName);

    }
}
