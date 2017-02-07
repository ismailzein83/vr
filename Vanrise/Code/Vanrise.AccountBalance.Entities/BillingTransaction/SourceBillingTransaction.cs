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

        public object TargetBEId
        {
            get { return this.BillingTransaction.AccountBillingTransactionId; }
        }

        public object SourceBEId
        {
            get { return this.BillingTransaction.SourceId; }
        }
    }
}
