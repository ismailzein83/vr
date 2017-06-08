using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BillingTransactionItem
    {
        public Decimal Amount { get; set; }
        public string CurrencySymbol { get; set; }
        public DateTime TransactionTime { get; set; }
        public string Notes { get; set; }
        public string TransactionTypeName { get; set; }
        public BillingTransactionItem() { }

        public IEnumerable<BillingTransactionItem> GetBillingTransactionItemSchema()
        {
            return null;
        }
    }
}
