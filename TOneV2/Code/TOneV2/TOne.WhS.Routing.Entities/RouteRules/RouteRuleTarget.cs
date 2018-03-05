using System;
using Vanrise.Entities;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleTarget : Vanrise.Rules.BaseRuleTarget, IRuleCodeTarget, IRuleSaleZoneTarget, IRuleCustomerTarget, IRuleRoutingProductTarget, IRuleCountryTarget
    {
        public string Code { get; set; }

        public long SaleZoneId { get; set; }

        public int CountryId { get; set; }

        public int? CustomerId { get; set; }

        public int? RoutingProductId { get; set; }

        public Decimal? SaleRate { get; set; }

        public bool BlockRoute { get; set; }

        #region Interfaces

        long? IRuleSaleZoneTarget.SaleZoneId
        {
            get { return this.SaleZoneId; }
        }

        int? IRuleCountryTarget.CountryId
        {
            get { return this.CountryId; }
        }

        int? IRuleCustomerTarget.CustomerId
        {
            get { return this.CustomerId; }
        }

        #endregion
    }
}