using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public enum BillingTransactionStatus { }
    public class BillingTransaction
    {
        public long AccountBillingTransactionId { get; set; }
        public String AccountId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid TransactionTypeId { get; set; }
        public Decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public DateTime TransactionTime { get; set; }
        public string Notes { get; set; }
        public string Reference { get; set; }
        public bool IsBalanceUpdated { get; set; }
        public string SourceId { get; set; }
    }
}
