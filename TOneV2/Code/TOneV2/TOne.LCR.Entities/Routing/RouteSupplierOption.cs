﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    [Serializable]
    public class RouteSupplierOption
    {
        static RouteSupplierOption()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RouteSupplierOption),
                "SupplierId", "SupplierZoneId", "Rate", "ServicesFlag", "Percentage", "IsBlocked", "Priority");
        }

        public string SupplierId { get; set; }

        public int SupplierZoneId { get; set; }

        public decimal Rate { get; set; }

        public short ServicesFlag { get; set; }

        public Int16? Percentage { get; set; }

        public bool IsBlocked { get; set; }

        public int Priority { get; set; }

        public RouteSupplierOption Clone()
        {
            return this.MemberwiseClone() as RouteSupplierOption;
        }
    }    
}
