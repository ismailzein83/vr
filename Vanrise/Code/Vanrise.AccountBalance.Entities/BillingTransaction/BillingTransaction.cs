using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

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
        public BillingTransactionSettings Settings { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSubtractedFromBalance { get; set; }
        public string SourceId { get; set; }
    }

    public class BillingTransactionSettings
    {
        public AttachmentFieldTypeEntityCollection Attachments { get; set; }
        public List<BillingTransactionUsageOverride> UsageOverrides { get; set; }
    }

    public class BillingTransactionUsageOverride
    {
        public Guid TransactionTypeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
