using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.AccountBalance.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.AccountBalance.Data;
using Vanrise.Common.Business;
using Vanrise.AccountBalance.Business;

namespace Vanrise.AccountBalance.BP.Activities
{
    #region Argument Classes

    public class ProcessNewBillingTransactionsInput
    {
        public BaseQueue<BillingTransactionBatch> InputQueue { get; set; }
        public AccountBalanceUpdateHandler AcountBalanceUpdateHandler { get; set; }
    }

    #endregion

    public sealed class ProcessNewBillingTransactions : DependentAsyncActivity<ProcessNewBillingTransactionsInput>
    {
     
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<BillingTransactionBatch>> InputQueue { get; set; }
       
        [RequiredArgument]
        public InArgument<AccountBalanceUpdateHandler> AcountBalanceUpdateHandler { get; set; }
      
        #endregion

        protected override void DoWork(ProcessNewBillingTransactionsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var counter = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (billingTransactionBatch) =>
                        {
                            counter += billingTransactionBatch.BillingTransactions.Count();
                            ProcessBillingTransactions(billingTransactionBatch.BillingTransactions,inputArgument.AcountBalanceUpdateHandler);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} New Billing Transactions Processed", counter);
        }
        protected override ProcessNewBillingTransactionsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessNewBillingTransactionsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                AcountBalanceUpdateHandler = this.AcountBalanceUpdateHandler.Get(context),
            };
        }
        private void ProcessBillingTransactions(List<BillingTransaction> billingTransactions, AccountBalanceUpdateHandler acountBalanceUpdateHandler)
        {
            acountBalanceUpdateHandler.AddAndUpdateLiveBalanceFromBillingTransction(billingTransactions);
        }
    }
}