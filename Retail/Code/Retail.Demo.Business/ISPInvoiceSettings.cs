using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.Demo.Business
{
    public class ISPInvoiceSettings : BaseRetailInvoiceTypeSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7B0E5F30-52BE-476A-B227-A7779C8ACAFA"); }
        }
        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(this.AccountBEDefinitionId, context.Invoice.PartnerId);

            switch (context.InfoType)
            {
                case "MailTemplate":
                    long accountId = Convert.ToInt32(financialAccountData.Account.AccountId);
                    var account = accountBEManager.GetAccount(this.AccountBEDefinitionId, accountId);
                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                    objects.Add("Operator", account);
                    objects.Add("Invoice", context.Invoice);
                    return objects;
                case "BankDetails":
                    {
                        #region BankDetails
                        return accountBEManager.GetBankDetailsIds(financialAccountData.Account.AccountId);
                        #endregion
                    }
            }
            return null;
        }

        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(this.AccountBEDefinitionId, context.PartnerId);
            context.PartnerCreationDate = financialAccountData.Account.CreatedTime;
        }

        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new ISPInvoiceGenerator(this.AccountBEDefinitionId);
        }

        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            switch (context.PartnerRetrievalType)
            {
                case PartnerRetrievalType.GetActive:
                case PartnerRetrievalType.GetAll:
                    FinancialAccountManager financialAccountManager = new FinancialAccountManager();
                    return financialAccountManager.GetAllFinancialAccountsIds(this.AccountBEDefinitionId);
                default:
                    return null;
            }
        }

        public override InvoicePartnerManager GetPartnerManager()
        {
            return new ISPInvoicePartnerSettings(this.AccountBEDefinitionId);
        }
    }
}
