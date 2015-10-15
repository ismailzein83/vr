using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteIdentifier : Vanrise.Rules.BaseRuleTargetIdentifier
    {
        public string Code { get; set; }

        public long SaleZoneId { get; set; }

        public int CustomerId { get; set; }

        public int? RoutingProductId { get; set; }
    }
}
