using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class AddContractInput
    {
        public Guid ContractTypeId { get; set; }

        public long CustomerId { get; set; }

        public long BillingAccountId { get; set; }

        public int RatePlanId { get; set; }

        public string MainResourceName { get; set; }

        public DateTime BET { get; set; }
    }
}
