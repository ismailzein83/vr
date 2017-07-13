using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;
using Retail.BusinessEntity.Entities;

namespace Retail.Zajil.MainExtensions.Convertors
{
    public class PaymentConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "Zajil Payment Convertor";
            }
        }
        public Guid TransactionTypeId { get; set; }
        public Guid ZajilCompanyExtendedInfoId { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
        public string SourceAccountIdColumn { get; set; }
        public string AmountColumn { get; set; }
        public string SourceIdColumn { get; set; }
        public string InvoiceSourceIdColumn { get; set; }
        public string TimeColumn { get; set; }
        public int CurrencyId { get; set; }

        public override void Initialize(ITargetBEConvertorInitializeContext context)
        {
            Dictionary<string, Account> initializationData = new Dictionary<string, Account>();
            var accounts = new AccountBEManager().GetAccounts(this.AccountBEDefinitionId);

            foreach (var account in accounts.Values)
            {
                if (account.Settings != null && account.Settings.Parts != null)
                {
                    AccountPart accountPart = null;
                    if (account.Settings.Parts.TryGetValue(ZajilCompanyExtendedInfoId, out accountPart))
                    {
                        ZajilCompanyExtendedInfo zajilCompanyExtendedInfo = accountPart.Settings as ZajilCompanyExtendedInfo;
                        string gpVoiceCustomerNo = zajilCompanyExtendedInfo.GPVoiceCustomerNo;
                        if (!string.IsNullOrEmpty(gpVoiceCustomerNo))
                            initializationData.GetOrCreateItem(gpVoiceCustomerNo, () => account);
                    }
                }
            }
            context.InitializationData = initializationData;
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            Dictionary<string, Account> accountsByGPInvoiceNumber = context.InitializationData as Dictionary<string, Account>;

            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            List<ITargetBE> transactionTargetBEs = new List<ITargetBE>();
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                string sourceId = row[this.SourceIdColumn] as string;
                try
                {
                    string accountId = row[this.SourceAccountIdColumn] as string;
                    accountId = string.IsNullOrEmpty(accountId) ? "" : accountId.Trim();
                    Account account = null;
                    if (!accountsByGPInvoiceNumber.TryGetValue(accountId, out account))
                    {
                        context.WriteBusinessTrackingMsg(LogEntryType.Error, "Failed to import Payment (SourceId: '{0}', SourceAccountId: '{1}') due to unavailable account.", sourceId, accountId);
                        continue;
                    }
                    SourceBillingTransaction sourceTransaction = new SourceBillingTransaction
                    {
                        BillingTransaction = new BillingTransaction
                        {
                            TransactionTypeId = TransactionTypeId,
                            SourceId = sourceId,
                            CurrencyId = this.CurrencyId,
                            AccountId = account.AccountId.ToString(),
                            TransactionTime = (DateTime)row[this.TimeColumn],
                            Amount = row[this.AmountColumn] != DBNull.Value ? (decimal)row[this.AmountColumn] : 0,
                            AccountTypeId = account.TypeId,
                            Reference = row["Applied_to_Doc_Number"] as string,
                            Notes = string.Format("Description: {0}, Document_Type_and_Number: {1}, PONUMBER: {2}, Invoive Description: {3}",
                                                    row["trx_desc"] as string,
                                                    row["Document_Type_and_Number"] as string,
                                                    row["PONUMBER"] as string,
                                                    row["Description"] as string)
                        },
                        InvoiceSourceId = row[InvoiceSourceIdColumn] as string
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
