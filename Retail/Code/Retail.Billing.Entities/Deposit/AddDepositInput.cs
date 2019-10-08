using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class AddDepositInput
    {
        public long BillingAccountId { get; set; }

        public long? ContractId { get; set; }

        public long? ContractServiceId { get; set; }

        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
    }
}
