﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class PriceListlUpdateOutput
    {
        public List<PriceListDetail> ListPriceListDetails { get; set; }
        public byte[] MaxTimeStamp { get; set; }
    }
}
