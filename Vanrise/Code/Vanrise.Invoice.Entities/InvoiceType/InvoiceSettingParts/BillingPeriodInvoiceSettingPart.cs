using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Entities
{
    public class BillingPeriodInvoiceSettingPart : InvoiceSettingPart
    {
        public bool FollowBillingPeriod { get; set; }
        public BillingPeriod BillingPeriod { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("D8730E6B-EF3E-4043-99FF-D5FA8F4EF812"); }
        }

    }
}
