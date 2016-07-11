using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;

namespace TOne.WhS.Sales.Data
{
    public interface IChangedSaleZoneRoutingProductDataManager : IDataManager, IBulkApplyDataManager<ChangedSaleZoneRoutingProduct>
    {
        long ProcessInstanceId { set; }

        void ApplyChangedSaleZoneRoutingProductsToDB(IEnumerable<ChangedSaleZoneRoutingProduct> changedSaleZoneRoutingProducts);
    }
}
