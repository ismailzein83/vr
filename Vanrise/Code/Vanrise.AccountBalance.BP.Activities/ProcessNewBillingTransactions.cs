using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.AccountBalance.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.AccountBalance.Data;

namespace Vanrise.AccountBalance.BP.Activities
{
    #region Argument Classes

    public class ProcessNewBillingTransactionsInput
    {
        public BaseQueue<BillingTransactionBatch> InputQueue { get; set; }
    }

    #endregion

    public sealed class ProcessNewBillingTransactions : DependentAsyncActivity<ProcessNewBillingTransactionsInput>
    {
     
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<BillingTransactionBatch>> InputQueue { get; set; }
    
        #endregion

        protected override void DoWork(ProcessNewBillingTransactionsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (billingTransactionBatch) =>
                        {
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Processing New Billing Transactions for accountID {0}", billingTransactionBatch.BillingTransactions.FirstOrDefault().AccountId);

                            ProcessBillingTransactions(billingTransactionBatch.BillingTransactions);
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finish Processing New Billing Transactions for accountID {0}", billingTransactionBatch.BillingTransactions.FirstOrDefault().AccountId);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });

        }
        protected override ProcessNewBillingTransactionsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessNewBillingTransactionsInput()
            {
                InputQueue = this.InputQueue.Get(context),
            };
        }

        private void ProcessBillingTransactions(List<BillingTransaction> billingTransactions)
        {
            decimal amount = 0;
            long accountId = -1;
            List<long> billingTransactionIds = new List<long>();
            foreach(var billingTransaction in billingTransactions)
            {
                billingTransactionIds.Add(billingTransaction.AccountBillingTransactionId);
                accountId = billingTransaction.AccountId;
                amount += billingTransaction.Amount;
            }
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            dataManager.UpdateBalance(accountId, billingTransactionIds, amount);
        }
    }
}