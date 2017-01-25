﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceType
    {
        public Guid InvoiceTypeId { get; set; }
        public string Name { get; set; }
        public InvoiceTypeSettings Settings { get; set; }
    }

}
