using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.Interconnect.Business
{
    public class InterconnectSettlementInvoiceSettings : BaseRetailInvoiceTypeSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("C0AF312F-C1AB-4F77-A9C9-663BBF9AD71F"); }
        }
        public Guid CustomerInvoiceTypeId { get; set; }
        public Guid SupplierInvoiceTypeId { get; set; }

        public override GenerationCustomSection GenerationCustomSection
        {
            get { return new InterconnectSettlementGenerationCustomSection(CustomerInvoiceTypeId, SupplierInvoiceTypeId); }
        }
        public override dynamic GetInfo(Vanrise.Invoice.Entities.IInvoiceTypeExtendedSettingsInfoContext context)
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
                        return accountBEManager.GetBankDetailsIds(this.AccountBEDefinitionId, financialAccountData.Account.AccountId);
                        #endregion
                    }
            }
            return null;
        }

        public override void GetInitialPeriodInfo(Vanrise.Invoice.Entities.IInitialPeriodInfoContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(this.AccountBEDefinitionId, context.PartnerId);
            context.PartnerCreationDate = financialAccountData.Account.CreatedTime;

        }

        public override Vanrise.Invoice.Entities.InvoiceGenerator GetInvoiceGenerator()
        {
            return new InterconnectSettlementInvoiceGenerator(this.AccountBEDefinitionId, this.CustomerInvoiceTypeId, this.SupplierInvoiceTypeId);
        }

        public override IEnumerable<string> GetPartnerIds(Vanrise.Invoice.Entities.IExtendedSettingsPartnerIdsContext context)
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

        public override Vanrise.Invoice.Entities.InvoicePartnerManager GetPartnerManager()
        {
            return new InterconnectPartnerSettings(this.AccountBEDefinitionId);
        }
    }
}
