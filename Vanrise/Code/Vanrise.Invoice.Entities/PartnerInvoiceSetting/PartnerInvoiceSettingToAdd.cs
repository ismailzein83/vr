﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class PartnerInvoiceSettingToAdd
    {
        public List<string> PartnerIds { get; set; }
        public Guid InvoiceSettingID { get; set; }
        public PartnerInvoiceSettingDetails Details { get; set; }
    }
}
