using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BillingTransactionMetaData
    {
        public String AccountId { get; set; }
        public Guid TransactionTypeId { get; set; }
        public Decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public DateTime TransactionTime { get; set; }
    }
}
