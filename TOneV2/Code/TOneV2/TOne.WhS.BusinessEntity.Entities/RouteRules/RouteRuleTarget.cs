using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRuleTarget : Vanrise.Rules.BaseRuleTarget, IRuleCodeTarget, IRuleSaleZoneTarget, IRuleCustomerTarget, IRuleRoutingProductTarget
    {
        public string Code { get; set; }

        public long SaleZoneId { get; set; }

        public int? CustomerId { get; set; }

        public int? RoutingProductId { get; set; }

        public Decimal? SaleRate { get; set; }

        public bool BlockRoute { get; set; }

        public List<RouteOptionRuleTarget> Options { get; set; }

        #region Interfaces

        long? IRuleSaleZoneTarget.SaleZoneId
        {
            get { return this.SaleZoneId; }
        }

        int? IRuleCustomerTarget.CustomerId
        {
            get { return this.CustomerId; }
        }

        #endregion
    }
}
