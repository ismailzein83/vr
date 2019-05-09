﻿using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOption : IRouteOptionOrderTarget, IRouteOptionPercentageTarget
    {
        static RPRouteOption()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RPRouteOption),
                "SupplierId", "SupplierRate", "Percentage", "OptionWeight", "SaleZoneId", "SupplierZoneMatchHasClosedRate");
        }

        public int SupplierId { get; set; }

        public long SaleZoneId { get; set; }

        public long? SupplierZoneId { get; set; }

        public HashSet<int> SupplierServicesIds { get; set; }

        public decimal? SupplierRate { get; set; }

        public int? Percentage { get; set; }

        public decimal OptionWeight { get; set; }

        public int SupplierServiceWeight { get; set; }

        public bool SupplierZoneMatchHasClosedRate { get; set; }

        public SupplierStatus SupplierStatus { get; set; }

        public bool IsForced { get; set; }

        long IRouteOptionOrderTarget.SaleZoneId { get { return this.SaleZoneId; } }

        long? IRouteOptionOrderTarget.SupplierZoneId { get { return null; } }
    }
}