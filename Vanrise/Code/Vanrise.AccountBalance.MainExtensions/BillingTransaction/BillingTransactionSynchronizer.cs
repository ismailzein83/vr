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
        public List<Guid> BillingTransactionTypeIds { get; set; }
        public Guid BalanceAccountTypeId { get; set; }
        public bool CheckExisting { get; set; }

        public override void Initialize(ITargetBESynchronizerInitializeContext context)
        {
            if (CheckExisting)
                context.InitializationData = new BillingTransactionManager().GetBillingTransactionsForSynchronizerProcess(BillingTransactionTypeIds, BalanceAccountTypeId);
        }
        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            if (CheckExisting)
            {
                Dictionary<string, Vanrise.AccountBalance.Entities.BillingTransaction> billingTransactions = context.InitializationData as Dictionary<string, Vanrise.AccountBalance.Entities.BillingTransaction>;
                billingTransactions.ThrowIfNull("billingTransactions", "");
                Vanrise.AccountBalance.Entities.BillingTransaction billingTransaction;
                if (billingTransactions.TryGetValue(context.SourceBEId.ToString(), out billingTransaction))
                {
                    context.TargetBE = new SourceBillingTransaction
                    {
                        BillingTransaction = billingTransaction
                    };
                    return true;
                }
            }
            return false;
        }

        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            context.TargetBE.ThrowIfNull("context.TargetBE", "");
            BillingTransactionManager billingTransactionManager = new BillingTransactionManager();
            foreach (var targetBillingTransaction in context.TargetBE)
            {
                long billingTransactionId = -1;
                SourceBillingTransaction sourceTransaction = targetBillingTransaction as SourceBillingTransaction;
                sourceTransaction.BillingTransaction.AccountTypeId = this.BalanceAccountTypeId;
                billingTransactionManager.TryAddBillingTransaction(sourceTransaction.BillingTransaction, out billingTransactionId);
            }
        }

        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {

        }
    }
}
