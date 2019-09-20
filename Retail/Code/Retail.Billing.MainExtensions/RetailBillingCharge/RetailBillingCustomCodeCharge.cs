using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.MainExtensions.RetailBillingCharge
{
    public class RetailBillingCustomCodeCharge : Retail.Billing.Entities.RetailBillingCharge
    {
        public Dictionary<string, Object> FieldValues { get; set; }
    }
}