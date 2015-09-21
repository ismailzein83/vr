using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class RouteRulesByCriteria
    {
        public abstract void SetSource(List<RouteRule> rules);

        public virtual RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, int saleZoneId)
        {
            return null;
        }
    }
}
