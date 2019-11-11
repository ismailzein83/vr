using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public abstract class RetailBillingCharge
    {
        public Guid RetailBillingChargeTypeId { get; set; }

        public abstract string GetDescription();
    }
}

