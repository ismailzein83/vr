using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRuleCriteria
    {
        public int? RoutingProductId { get; set; }

        public List<RouteRuleCriteriaCode> Codes { get; set; }

        public List<string> ExcludedCodes { get; set; }

        public List<long> ZoneIds { get; set; }

        public List<long> ExcludedZoneIds { get; set; }

        public List<int> CustomerIds { get; set; }

        public List<int> ExcludedCustomerIds { get; set; }
    }

    public class RouteRuleCriteriaCode
    {
        public string Code { get; set; }

        public bool WithSubCodes { get; set; }
    }
}
