using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionRuleTarget : Vanrise.Rules.BaseRuleTarget,
        IRouteOptionOrderTarget, IRouteOptionFilterTarget, IRouteOptionPercentageTarget,
        IRuleSupplierTarget, IRuleSupplierZoneTarget, IRuleCodeTarget, IRuleSaleZoneTarget, IRuleCustomerTarget, IRuleRoutingProductTarget
    {
        public RouteRuleTarget RouteTarget { get; set; }

        public int SupplierId { get; set; }

        public string SupplierCode { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal SupplierRate { get; set; }

        public Decimal? Percentage { get; set; }

        public bool BlockOption { get; set; }

        public bool FilterOption { get; set; }

        public int? ExecutedRuleId { get; set; }

        public HashSet<int> ExactSupplierServiceIds { get; set; }

        public int SupplierServiceWeight { get; set; }

        public int NumberOfTries { get; set; }

        #region Interfaces

        string IRuleCodeTarget.Code
        {
            get { return this.RouteTarget.Code; }
        }

        long? IRuleSaleZoneTarget.SaleZoneId
        {
            get { return this.RouteTarget.SaleZoneId; }
        }

        int? IRuleCustomerTarget.CustomerId
        {
            get { return this.RouteTarget.CustomerId; }
        }
        int? IRuleRoutingProductTarget.RoutingProductId
        {
            get { return this.RouteTarget.RoutingProductId; }
        }

        int? IRuleSupplierTarget.SupplierId
        {
            get { return this.SupplierId; }
        }

        long? IRuleSupplierZoneTarget.SupplierZoneId
        {
            get { return this.SupplierZoneId; }
        }

        public decimal OptionWeight { get; set; }
        
        #endregion


        long? IRouteOptionOrderTarget.SaleZoneId
        {
            get { return this.RouteTarget.SaleZoneId; }
        }

        long? IRouteOptionOrderTarget.SupplierZoneId
        {
            get { return this.SupplierZoneId; }
        }
    }
}