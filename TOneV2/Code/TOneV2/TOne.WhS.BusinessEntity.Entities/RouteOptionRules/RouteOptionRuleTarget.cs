﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteOptionRuleTarget : Vanrise.Rules.BaseRuleTarget, IRuleSupplierTarget, IRuleSupplierZoneTarget, IRuleCodeTarget, IRuleSaleZoneTarget, IRuleCustomerTarget, IRuleRoutingProductTarget
    {
        public RouteRuleTarget RouteTarget { get; set; }

        public int SupplierId { get; set; }

        public string SupplierCode { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal SupplierRate { get; set; }

        public Decimal Percentage { get; set; }

        public bool BlockOption { get; set; }

        public bool FilterOption { get; set; }

        public int? ExecutedRuleId { get; set; }

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

        #endregion
    }
}
