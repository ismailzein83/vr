using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class PartnerInvoiceSettingDetail
    {
        public PartnerInvoiceSetting Entity { get; set; }
        public string PartnerName { get; set; }
        public DateTime? BED { get; set; }
        public DateTime? EED { get; set; }
        public string StatusDescription { get; set; }

    }
}
