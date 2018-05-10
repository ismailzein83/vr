using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.Common;
namespace Retail.BusinessEntity.Business
{
    public class AccountInvToAccBalanceRelationDefinitionExtendedSettings : InvToAccBalanceRelationDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("D879B75A-7F13-4543-8EDE-961327CB3E33"); }
        }
        public Guid AccountBEDefinitionId { get; set; }
        public override List<InvoiceAccountInfo> GetBalanceInvoiceAccounts(IInvToAccBalanceRelGetBalanceInvoiceAccountsContext context)
        {

            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = new FinancialAccountManager().GetFinancialAccountData(this.AccountBEDefinitionId, context.AccountId);
            financialAccountData.ThrowIfNull("financialAccountData");
            List<InvoiceAccountInfo> invoiceAccountInfo = new List<InvoiceAccountInfo>();
            FinancialAccountDefinitionManager financialAccountDefinitionManager = new FinancialAccountDefinitionManager();
            var accountBalanceSettings = financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccountData.FinancialAccount.FinancialAccountDefinitionId);

            if (accountBalanceSettings.InvoiceTypes != null)
            {
                foreach(var invoiceType in  accountBalanceSettings.InvoiceTypes)
                {
                    invoiceAccountInfo.Add(new InvoiceAccountInfo
                    {
                        InvoiceTypeId = invoiceType.InvoiceTypeId,
                        PartnerId = context.AccountId
                    });
                }
            }
            return invoiceAccountInfo;
        }

        public override List<BalanceAccountInfo> GetInvoiceBalanceAccounts(IInvToAccBalanceRelGetInvoiceBalanceAccountsContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = new FinancialAccountManager().GetFinancialAccountData(this.AccountBEDefinitionId, context.PartnerId);
            financialAccountData.ThrowIfNull("financialAccountData");
            List<BalanceAccountInfo> balanceAccountInfo = new List<BalanceAccountInfo>();
            FinancialAccountDefinitionManager financialAccountDefinitionManager = new FinancialAccountDefinitionManager();
            var accountBalanceSettings = financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccountData.FinancialAccount.FinancialAccountDefinitionId);
            if (accountBalanceSettings.BalanceAccountTypeId.HasValue)
            {
                balanceAccountInfo.Add(new BalanceAccountInfo
                {
                    AccountTypeId = accountBalanceSettings.BalanceAccountTypeId.Value,
                    AccountId = context.PartnerId
                });
            }
            return balanceAccountInfo;
        }
    }
}
