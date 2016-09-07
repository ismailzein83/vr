﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceItemConcatenatedPartContext : IInvoiceItemConcatenatedPartContext
    {
        public dynamic InvoiceItemDetails { get; set; }
        public string  CurrentItemSetName { get; set; }
    }
}
