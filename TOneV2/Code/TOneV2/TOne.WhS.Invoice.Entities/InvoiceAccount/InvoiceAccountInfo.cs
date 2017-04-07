using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceAccountInfo
    {
        public int InvoiceAccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TimeZoneId { get; set; }
    }

    public enum InvoiceAccountEffectiveStatus { Recent = 0, Current = 1, Future = 2 }

    public enum InvoiceAccountCarrierType
    {
        [Description("Prof")]
        Profile = 0,

        [Description("Acc")]
        Account = 1
    }
}
