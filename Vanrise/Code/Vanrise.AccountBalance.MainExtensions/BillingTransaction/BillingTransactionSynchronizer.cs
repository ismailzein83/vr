using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Invoice.Business;

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
        public bool UpdateInvoicePaidDate { get; set; }
        public Guid InvoiceTypeId { get; set; }

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
            var accountManager = new AccountManager();
            var billingTransactionTypeManager = new BillingTransactionTypeManager();
            var currencyManager = new Vanrise.Common.Business.CurrencyManager();
            BillingTransactionManager billingTransactionManager = new BillingTransactionManager();
            foreach (var targetBillingTransaction in context.TargetBE)
            {
                SourceBillingTransaction sourceTransaction = targetBillingTransaction as SourceBillingTransaction;
                var billingTransaction = sourceTransaction.BillingTransaction;
                string transactionType = billingTransactionTypeManager.GetBillingTransactionTypeName(billingTransaction.TransactionTypeId);
                try
                {
                    long billingTransactionId;
                    sourceTransaction.BillingTransaction.AccountTypeId = this.BalanceAccountTypeId;
                    billingTransactionManager.TryAddBillingTransaction(sourceTransaction.BillingTransaction, out billingTransactionId);
                    string accountName = accountManager.GetAccountName(billingTransaction.AccountTypeId, billingTransaction.AccountId);
                    string currencyName = currencyManager.GetCurrencySymbol(billingTransaction.CurrencyId);
                    context.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, "New {0} Transaction imported for '{1}'. Transaction Amount is {2} {3}",
                        transactionType, accountName, billingTransaction.Amount, currencyName);

                    if (this.UpdateInvoicePaidDate)
                    {
                        InvoiceManager invoiceManager = new InvoiceManager();
                        bool updated = false;

                        if (sourceTransaction.InvoiceId.HasValue)
                            updated = invoiceManager.UpdateInvoicePaidDateById(InvoiceTypeId, sourceTransaction.InvoiceId.Value, billingTransaction.TransactionTime);
                        else
                            updated = invoiceManager.UpdateInvoicePaidDateBySourceId(InvoiceTypeId, sourceTransaction.InvoiceSourceId, billingTransaction.TransactionTime);

                        if (updated)
                        {
                            context.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, "Invoice Paid Date {1} with Source Id {0} is updated",
                                                    sourceTransaction.InvoiceSourceId, billingTransaction.TransactionTime);
                        }
                        else
                        {
                            context.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Warning, "Invoice Paid Date {1} with Source Id {0} was not updated",
                                                 sourceTransaction.InvoiceSourceId, billingTransaction.TransactionTime);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var finalException = Utilities.WrapException(ex, String.Format("Failed to import {0} Transaction. Source Transaction Id '{1}'", transactionType, billingTransaction.SourceId));
                    context.WriteBusinessHandledException(finalException);
                }
            }
        }

        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {

        }
    }
}
