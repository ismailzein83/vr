﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceDetail
    {
        public Invoice Entity { get; set; }
        public string PartnerName { get; set; }
        public List<string> SectionsTitle { get; set; }
        public List<InvoiceGridAction> ActionTypeNames { get; set; }
        public Boolean Paid { get; set; }
    }
}
