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
        public string TimeColumn { get; set; }
        public int CurrencyId { get; set; }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            List<ITargetBE> transactionTargetBEs = new List<ITargetBE>();
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                AccountBEManager accountBeManager = new AccountBEManager();

                SourceBillingTransaction sourceTransaction = new SourceBillingTransaction
                {
                    BillingTransaction = new BillingTransaction
                    {
                        TransactionTypeId = TransactionTypeId,
                        SourceId = row["PaymentId"].ToString(),
                        CurrencyId = this.CurrencyId,
                        AccountId = accountBeManager.GetAccountBySourceId(AccountBEDefinitionId, row[this.SourceAccountIdColumn].ToString()).AccountId.ToString(),
                        TransactionTime = (DateTime)row[this.TimeColumn],
                        Amount = (decimal)row[this.AmountColumn]
                    }
                };
                transactionTargetBEs.Add(sourceTransaction);
            }
            context.TargetBEs = transactionTargetBEs;
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {

        }
    }
}
