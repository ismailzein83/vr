﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceTypeInfo
    {
        public Guid InvoiceTypeId { get; set; }
        public string Name { get; set; }
        public string NameResourceKey { get; set; }
    }
}
