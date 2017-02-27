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
        public Guid AccountBEDefinitionId { get; set; }
        public string SourceAccountIdColumn { get; set; }
        public string AmountColumn { get; set; }
        public string SourceIdColumn { get; set; }
        public string TimeColumn { get; set; }
        public int CurrencyId { get; set; }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            List<ITargetBE> transactionTargetBEs = new List<ITargetBE>();
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                string sourceId = row[this.SourceIdColumn] as string;
                try
                {
                    AccountBEManager accountBeManager = new AccountBEManager();
                    var account = accountBeManager.GetAccountBySourceId(AccountBEDefinitionId, row[this.SourceAccountIdColumn].ToString());
                    SourceBillingTransaction sourceTransaction = new SourceBillingTransaction
                    {
                        BillingTransaction = new BillingTransaction
                        {
                            TransactionTypeId = TransactionTypeId,
                            SourceId = sourceId,
                            CurrencyId = this.CurrencyId,
                            AccountId = account.AccountId.ToString(),
                            TransactionTime = (DateTime)row[this.TimeColumn],
                            Amount = (decimal)row[this.AmountColumn],
                            AccountTypeId = account.TypeId,
                            Reference = row["Applied_to_Doc_Number"] as string,
                            Notes = string.Format("Description: {0}, Document_Type_and_Number: {1}, PONUMBER: {2}, Invoive Description: {3}",
                                                    row["trx_desc"] as string,
                                                    row["Document_Type_and_Number"] as string,
                                                    row["PONUMBER"] as string,
                                                    row["Description"] as string)
                        }
                    };
                    transactionTargetBEs.Add(sourceTransaction);
                }
                catch(Exception ex)
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
