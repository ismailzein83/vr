using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleEntityRoutingProductDataManager : IDataManager
    {
        IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProducts(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingCustomerInfo> customerInfos);

        IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProducts(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingCustomerInfo> customerInfos);
    }
}
