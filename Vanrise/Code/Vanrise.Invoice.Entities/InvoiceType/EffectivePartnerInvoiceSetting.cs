using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class EffectivePartnerInvoiceSetting
    {
        public InvoiceSetting InvoiceSetting { get; set; }
        public PartnerInvoiceSetting PartnerInvoiceSetting { get; set; }
    }
}
