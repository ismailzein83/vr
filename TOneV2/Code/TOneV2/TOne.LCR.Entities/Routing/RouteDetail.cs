﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{

    public class RouteDetail
    {
        public string CustomerID { get; set; }

        public string Code { get; set; }

        public int SaleZoneId { get; set; }

        public decimal Rate { get; set; }

        public short ServicesFlag { get; set; }

        public RouteOptions Options { get; set; }
    }

    public class RouteDetailBatch
    {
        public List<RouteDetail> Routes { get; set; }
    }
}