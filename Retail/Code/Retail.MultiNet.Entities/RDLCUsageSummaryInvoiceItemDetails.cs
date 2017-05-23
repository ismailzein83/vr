﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.Entities
{
    public class RDLCUsageSummaryInvoiceItemDetails
    {
        public string UsageDescription { get; set; }
        public int Quantity { get; set; }
        public int TotalDuration { get; set; }
        public Decimal NetAmount { get; set; }
        public RDLCUsageSummaryInvoiceItemDetails()
        {
                
        }
        public IEnumerable<RDLCUsageSummaryInvoiceItemDetails> GetRDLCUsageSummaryInvoiceItemDetailsSchema()
        {
            return null;
        }
    }
}
