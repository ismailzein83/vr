using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface IExcludedItemsDataManager : IDataManager
    {
        void BulkInsertExcludedItems(List<ExcludedItem> excludedItems);
        IEnumerable<ExcludedItem> GetAllExcludedItems(ExcludedItemsQuery query);
        IEnumerable<long> GetExcludedItemsProcessInstanceIds(IEnumerable<long> subscribersProcessInstanceIds);
    }
}
