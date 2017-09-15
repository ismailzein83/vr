﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOption : IRouteOptionOrderTarget, IRouteOptionFilterTarget, IRouteOptionPercentageTarget
    {
        static RPRouteOption()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RPRouteOption),
                "SupplierId", "SupplierRate", "Percentage", "OptionWeight", "SaleZoneId", "SupplierZoneMatchHasClosedRate");
        }

        public int SupplierId { get; set; }

        public Decimal SupplierRate { get; set; }

        public int? Percentage { get; set; }

        public decimal OptionWeight { get; set; }

        public long SaleZoneId { get; set; }

        public SupplierStatus SupplierStatus { get; set; }

        public int SupplierServiceWeight { get; set; }

        long? IRouteOptionOrderTarget.SaleZoneId
        {
            get { return this.SaleZoneId; }
        }

        long? IRouteOptionOrderTarget.SupplierZoneId
        {
            get { return null; }
        }

        public bool SupplierZoneMatchHasClosedRate { get; set; }
    }
}
