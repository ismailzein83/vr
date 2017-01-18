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
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                CurrencyManager currencyManager = new CurrencyManager();
                AccountBEManager accountBeManager = new AccountBEManager();

                SourceBillingTransaction sourceTransaction = new SourceBillingTransaction
                {
                    BillingTransaction = new BillingTransaction
                    {
                        TransactionTypeId = TransactionTypeId,
                        SourceId = row["PaymnetId"].ToString(),
                        CurrencyId = currencyManager.GetSystemCurrency().CurrencyId,
                        AccountId = accountBeManager.GetAccountBySourceId(AccountBEDefinitionId, row["AccountId"].ToString()).AccountId
                    }
                };
            }
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
