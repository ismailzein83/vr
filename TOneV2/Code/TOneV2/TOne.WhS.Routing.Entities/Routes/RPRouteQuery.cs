﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public abstract class BaseRPRouteQuery
    {
        public int RoutingDatabaseId { get; set; }

        public int NumberOfOptions { get; set; }

        public List<int> RoutingProductIds { get; set; }

        public int? SimulatedRoutingProductId { get; set; }

        public int? SellingNumberPlanId { get; set; }

        public List<int> SaleZoneIds { get; set; }

        public RouteStatus? RouteStatus { get; set; }

        public int LimitResult { get; set; }

        public int? CustomerId { get; set; }

        public bool ShowInSystemCurrency { get; set; }

        public bool IncludeBlockedSuppliers { get; set; }

        public decimal? MaxSupplierRate { get; set; }
    }

    public class RPRouteQueryByZone : BaseRPRouteQuery
    {
        public Guid PolicyConfigId { get; set; }
    }

    public class RPRouteQueryByCode : BaseRPRouteQuery
    {
        public string CodePrefix { get; set; }
    }
}
