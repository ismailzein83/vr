using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;

namespace Vanrise.AccountBalance.MainExtensions.BillingTransaction
{
    public class BillingTransactionSynchronizer : TargetBESynchronizer
    {
        public override string Name
        {
            get
            {
                return "Billing Transaction Synchronizer";
            }
        }
        public Guid BalanceAccountTypeId { get; set; }
        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            return false;
        }

        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            context.TargetBE.ThrowIfNull("context.TargetBE", "");
            BillingTransactionManager billingTransactionManager = new BillingTransactionManager();
            foreach (var targetBillingTransaction in context.TargetBE)
            {
                SourceBillingTransaction sourceTransaction = targetBillingTransaction as SourceBillingTransaction;
                sourceTransaction.BillingTransaction.AccountTypeId = this.BalanceAccountTypeId;
                billingTransactionManager.AddBillingTransaction(sourceTransaction.BillingTransaction);
            }
        }

        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {

        }
    }
}
