﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierRateDetail
    {
        public SupplierRate Entity { get; set; }

        public string SupplierZoneName { get; set; }

        public decimal DisplayedRate { get; set; }

        public string DisplayedCurrency { get; set; }
        public int SupplierId { get; set; }
    }
}
