using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class SourceBillingTransaction : ITargetBE
    {
        public BillingTransaction BillingTransaction { get; set; }

        public string InvoiceSourceId { get; set; }
        public object TargetBEId
        {
            get { return this.BillingTransaction.AccountBillingTransactionId > 0 ? (object)this.BillingTransaction.AccountBillingTransactionId : null; }
        }

        public object SourceBEId
        {
            get { return this.BillingTransaction.SourceId; }
        }
    }
}
