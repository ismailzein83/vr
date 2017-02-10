using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using System.Threading;

namespace Vanrise.AccountBalance.BP.Activities
{

    #region Argument Classes
    public class LoadNewBillingTransactionsInput
    {
        public Guid AccountTypeId { get; set; }
        public BaseQueue<BillingTransactionBatch> OutputQueue { get; set; }
    }
    #endregion

    public sealed class LoadNewBillingTransactions : BaseAsyncActivity<LoadNewBillingTransactionsInput>
    {

        #region Arguments
         [RequiredArgument]
        public InArgument<Guid> AccountTypeId { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<BillingTransactionBatch>> OutputQueue { get; set; }
        #endregion

        protected override void DoWork(LoadNewBillingTransactionsInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading New Billing Transactions ...");

            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            String accountId = null;
            var list = new List<BillingTransaction>();
            dataManager.GetBillingTransactionsByBalanceUpdated(inputArgument.AccountTypeId, (billingTransaction) =>
            {
                if (accountId == null)
                    accountId = billingTransaction.AccountId;

                if (billingTransaction.AccountId != accountId)
                {
                    var billingTransactionBatch = new BillingTransactionBatch() { BillingTransactions = list };
                    inputArgument.OutputQueue.Enqueue(billingTransactionBatch);
                    list = new List<BillingTransaction>();
                    accountId = billingTransaction.AccountId;
                }
                list.Add(billingTransaction);
            });

            if (list.Count > 0)
            {
                var billingTransactionBatch = new BillingTransactionBatch() { BillingTransactions = list };
                inputArgument.OutputQueue.Enqueue(billingTransactionBatch);
            }
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Finish Loading {0} New Billing Transactions.",list.Count()));
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<BillingTransactionBatch>());
            base.OnBeforeExecute(context, handle);
        }
        protected override LoadNewBillingTransactionsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadNewBillingTransactionsInput()
            {
                AccountTypeId = this.AccountTypeId.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
