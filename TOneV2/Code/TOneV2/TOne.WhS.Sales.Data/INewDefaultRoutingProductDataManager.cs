using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;

namespace TOne.WhS.Sales.Data
{
    public interface INewDefaultRoutingProductDataManager : IDataManager, IBulkApplyDataManager<NewDefaultRoutingProduct>
    {
        long ProcessInstanceId { set; }

        void ApplyNewDefaultRoutingProductsToDB(IEnumerable<NewDefaultRoutingProduct> newDefaultRoutingProducts);
    }
}
