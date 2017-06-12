﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class GeneratedInvoiceBillingTransaction
    {
        public Guid AccountTypeId { get; set; }
        public String AccountId { get; set; }
        public Guid TransactionTypeId { get; set; }
        public Decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public DateTime TransactionTime { get; set; }
        public string Notes { get; set; }
        public string Reference { get; set; }
        public GeneratedInvoiceBillingTransactionSettings Settings { get; set; }
    }

    public class GeneratedInvoiceBillingTransactionSettings
    {
        public List<GeneratedInvoiceBillingTransactionUsageOverride> UsageOverrides { get; set; }
    }

    public class GeneratedInvoiceBillingTransactionUsageOverride
    {
        public Guid TransactionTypeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
