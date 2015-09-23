using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByProductZone<T> : RouteRulesByTwoIds<T, int, long> where T : IRouteCriteria
    {
        protected override bool IsRuleMatched(T rule, out IEnumerable<int> ids1, out IEnumerable<long> ids2)
        {
             if (rule.RouteCriteria.RoutingProductId.HasValue && rule.RouteCriteria.HasZoneFilter())
             {
                 ids1 = new List<int> { rule.RouteCriteria.RoutingProductId.Value };
                 SaleZoneManager saleZoneManager = new SaleZoneManager();
                 ids2 = saleZoneManager.GetSaleZoneIds(rule.RouteCriteria.SaleZoneGroupConfigId.Value, rule.RouteCriteria.SaleZoneGroupSettings);
                 //validate that rule zones are available in routing product's zones. and remove the ones that are not there
                 RoutingProductManager routingProductManager = new RoutingProductManager();
                 RoutingProduct routingProduct = routingProductManager.GetRoutingProduct(rule.RouteCriteria.RoutingProductId.Value);
                 if(routingProduct.Settings.SaleZoneGroupConfigId.HasValue)
                 {
                     List<long> routingProductZoneIds = saleZoneManager.GetSaleZoneIds(routingProduct.Settings.SaleZoneGroupConfigId.Value, routingProduct.Settings.SaleZoneGroupSettings);
                     ids2 = ids2.Where(id => routingProductZoneIds.Contains(id));                 
                 }
                 return ids2.Count() > 0;
             }
             else
             {
                 ids1 = null;
                 ids2 = null;
                 return false;
             }
        }

        protected override bool AreIdsAvailable(int? customerId, int? productId, string code, long saleZoneId, out int id1, out long id2)
        {
            if (productId != null)
            {
                id1 = productId.Value;
                id2 = saleZoneId;
                return true;
            }
            else
            {
                id1 = 0;
                id2 = 0;
                return false;
            }
        }
    }
}
