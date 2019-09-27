using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using Vanrise.Invoice.Entities;

namespace Retail.QualityNet.Business
{
    public class QualitynetInvoiceSettings : BaseRetailInvoiceTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("D8AF155C-4303-491B-B8DD-553DEEEB9C68"); } }

        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            var financialAccountData = new FinancialAccountManager().GetFinancialAccountData(this.AccountBEDefinitionId, context.Invoice.PartnerId);

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
                        return accountBEManager.GetBankDetailsIds(this.AccountBEDefinitionId, financialAccountData.Account.AccountId);
                        #endregion
                    }
            }
            return null;
        }

        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            var financialAccountData = new FinancialAccountManager().GetFinancialAccountData(this.AccountBEDefinitionId, context.PartnerId);
            context.PartnerCreationDate = financialAccountData.Account.CreatedTime;
        }

        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new QualitynetInvoiceGenerator(this.AccountBEDefinitionId);
        }

        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            switch (context.PartnerRetrievalType)
            {
                case PartnerRetrievalType.GetActive:
                case PartnerRetrievalType.GetAll:
                    return new FinancialAccountManager().GetAllFinancialAccountsIds(this.AccountBEDefinitionId);
                default:
                    return null;
            }
        }

        public override InvoicePartnerManager GetPartnerManager()
        {
            return new QualitynetInvoicePartnerSettings(this.AccountBEDefinitionId);
        }
    }
}