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
        public BaseQueue<BillingTransactionBatch> OutputQueue { get; set; }
    }
    #endregion

    public sealed class LoadNewBillingTransactions : BaseAsyncActivity<LoadNewBillingTransactionsInput>
    {
        [RequiredArgument]
        public InOutArgument<BaseQueue<BillingTransactionBatch>> OutputQueue { get; set; }
        protected override void DoWork(LoadNewBillingTransactionsInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading New Billing Transactions ...");
          
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            long accountId = -1;
            long totalCount = 0;
            var list = new List<BillingTransaction>();
            dataManager.GetBillingTransactionsByBalanceUpdated( (billingTransaction) =>
            {
                if (accountId == -1)
                    accountId = billingTransaction.AccountId;
              
                if (billingTransaction.AccountId != accountId)
                {
                    var billingTransactionBatch = new BillingTransactionBatch() { BillingTransactions = list };
                    totalCount = list.Count();
                    handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} New Billing Transactions of AccountID {1} loaded.", totalCount, accountId);
                    inputArgument.OutputQueue.Enqueue(billingTransactionBatch);
                    list = new List<BillingTransaction>();
                    accountId = billingTransaction.AccountId;
                }

                list.Add(billingTransaction);
            });
         
            if(list.Count>0)
            {
                var billingTransactionBatch = new BillingTransactionBatch() { BillingTransactions = list };
                totalCount = list.Count();
                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} New Billing Transactions of AccountID {1} loaded.", totalCount, accountId);
                inputArgument.OutputQueue.Enqueue(billingTransactionBatch);
            }
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
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
