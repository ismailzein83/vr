using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BillingTransactionDetail
    {
        public BillingTransaction Entity { get; set; }
        public string CurrencyDescription { get; set; }
        public AccountInfo AccountInfo { get; set; }
        public string  TransactionTypeDescription { get; set; }
        public double? Debit { get; set; }
        public double? Credit { get; set; }

    }
}
