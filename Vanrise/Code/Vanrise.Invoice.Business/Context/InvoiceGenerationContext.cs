﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceGenerationContext : IInvoiceGenerationContext
    {
        public string PartnerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public dynamic CustomSectionPayload { get; set; }
        public GeneratedInvoice Invoice { get; set; }
    }
}
