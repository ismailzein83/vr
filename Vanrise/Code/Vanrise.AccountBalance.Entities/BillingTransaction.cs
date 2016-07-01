﻿using System;
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

        public long AccountId { get; set; }

        public int TransactionTypeId { get; set; }

        public Decimal Amount { get; set; }

        public int CurrencyId { get; set; }

        public DateTime TransactionTime { get; set; }

        public string Notes { get; set; }
    }
}
