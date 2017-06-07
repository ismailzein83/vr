using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.MultiNet.Business
{
    public class MultiNetPartnerCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("E1F20103-90C1-4290-B0B2-22A1B642A2FE"); }
        }
        public Retail.BusinessEntity.Entities.AccountCondition AccountCondition { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(AccountBEDefinitionId,context.Invoice.PartnerId);
            return accountBEManager.EvaluateAccountCondition(financialAccountData.Account, AccountCondition);
        }
    }
}
