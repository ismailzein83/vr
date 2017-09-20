using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Retail.MultiNet.Business.Convertors
{
    public class PaymentConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "MultiNet Payment Convertor";
            }
        }
        public Guid TransactionTypeId { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
        public string SourceAccountIdColumn { get; set; }
        public string AmountColumn { get; set; }
        public string InvoiceSourceIdColumn { get; set; }
        public string PaymentDateColumn { get; set; }
        public string ReferenceColumnName { get; set; }
        public string CurrencyColumnName { get; set; }
        public string SourceIdColumnName { get; set; }


        public override void Initialize(ITargetBEConvertorInitializeContext context)
        {
            context.InitializationData = new AccountBEManager().GetCachedAccountsBySourceId(this.AccountBEDefinitionId);
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            Dictionary<string, Account> accountsBySourceId = context.InitializationData as Dictionary<string, Account>;

            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            List<ITargetBE> transactionTargetBEs = new List<ITargetBE>();
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                long sourceId = (Int64)row[this.SourceIdColumnName];
                try
                {
                    string currencySourceId = (row[CurrencyColumnName] as string).Trim();
                    CurrencyManager currencyManager = new CurrencyManager();
                    Currency currency = currencyManager.GetCurrencyBySymbol(currencySourceId);
                    currency.ThrowIfNull("currency", currencySourceId);

                    string accountId = string.Format("Customer_{0}", (row[SourceAccountIdColumn] as string).Trim());

                    Account account = null;
                    if (!accountsBySourceId.TryGetValue(accountId, out account))
                    {
                        context.WriteBusinessTrackingMsg(LogEntryType.Error, "Failed to import Payment (SourceId: '{0}', SourceAccountId: '{1}') due to unavailable account.", sourceId, accountId);
                        continue;
                    }
                    DateTime transactionTime = (DateTime)row[this.PaymentDateColumn];
                    FinancialAccountData financialAccountData = null;
                    new FinancialAccountManager().TryGetFinancialAccount(AccountBEDefinitionId, account.AccountId, true, transactionTime, out financialAccountData);
                    financialAccountData.ThrowIfNull("financialAccountData");
                    SourceBillingTransaction sourceTransaction = new SourceBillingTransaction
                    {
                        BillingTransaction = new BillingTransaction
                        {
                            TransactionTypeId = TransactionTypeId,
                            SourceId = sourceId.ToString(),
                            CurrencyId = currency.CurrencyId,
                            AccountId = financialAccountData.FinancialAccountId,
                            TransactionTime = transactionTime,
                            Amount = row[this.AmountColumn] != DBNull.Value ? (decimal)row[this.AmountColumn] : 0,
                            AccountTypeId = account.TypeId,
                            Reference = (row[ReferenceColumnName] as string).Trim()
                        },
                        InvoiceSourceId = (row[InvoiceSourceIdColumn] as string).Trim()
                    };
                    transactionTargetBEs.Add(sourceTransaction);
                }
                catch (Exception ex)
                {
                    var finalException = Utilities.WrapException(ex, String.Format("Failed to import Payment (SourceId: '{0}') due to conversion error", sourceId));
                    context.WriteBusinessHandledException(finalException);
                }
            }
            context.TargetBEs = transactionTargetBEs;
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
            SourceBillingTransaction existingBe = context.ExistingBE as SourceBillingTransaction;
            SourceBillingTransaction newBe = context.NewBE as SourceBillingTransaction;

            SourceBillingTransaction finalBe = new SourceBillingTransaction
            {
                BillingTransaction = existingBe.BillingTransaction
            };

            context.FinalBE = finalBe;
        }
    }

}
