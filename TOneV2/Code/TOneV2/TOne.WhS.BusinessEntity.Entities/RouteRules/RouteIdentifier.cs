using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteIdentifier : Vanrise.Rules.BaseRuleTarget, IRuleSaleZoneTarget, IRuleCodeTarget, IRuleCustomerTarget, IRuleRoutingProductTarget
    {
        public string Code { get; set; }

        public long SaleZoneId { get; set; }

        public int CustomerId { get; set; }

        public int? RoutingProductId { get; set; }

        long? IRuleSaleZoneTarget.SaleZoneId
        {
            get { return this.SaleZoneId; }
        }

        int? IRuleCustomerTarget.CustomerId
        {
            get { return this.CustomerId; }
        }
    }
}
