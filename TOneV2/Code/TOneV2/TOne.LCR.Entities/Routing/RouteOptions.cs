﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    [Serializable]
    public class RouteOptions
    {
        public bool IsBlock { get; set; }

        public List<RouteSupplierOption> SupplierOptions { get; set; }
    }
}
