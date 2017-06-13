using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.MultiNet.Business
{
    public class MultiNetSubscriberInvoiceSettings : InvoiceTypeExtendedSettings
    {
        public Guid AccountBEDefinitionId { get; set; }
        public Guid CompanyExtendedInfoPartdefinitionId { get; set; }
        public List<Guid> SalesTaxChargeableEntities { get { return new List<Guid> { new Guid("fc8a8acc-5c10-49f0-95fd-b7e95ed5db80"), new Guid("711039b3-92ee-4cb9-80e5-ac6354452c8e"), new Guid("f062a145-a311-4629-a96d-d770c34c7da6") }; } }
        public List<Guid> WHTaxChargeableEntities { get { return new List<Guid> { new Guid("fc8a8acc-5c10-49f0-95fd-b7e95ed5db80"), new Guid("711039b3-92ee-4cb9-80e5-ac6354452c8e") }; } }
        public Guid InComingChargeableEntity { get { return new Guid("f062a145-a311-4629-a96d-d770c34c7da6"); } }
        public Guid OutGoingChargeableEntity { get { return new Guid("fc8a8acc-5c10-49f0-95fd-b7e95ed5db80"); } }
        public Guid SalesTaxRuleDefinitionId { get { return new Guid("d15d107e-64f0-4caa-bc46-ba58ed30e848"); } }
        public Guid WHTaxRuleDefinitionId { get { return new Guid("3f6002b5-3e4b-4300-b676-e11176b54782"); } }
        public Guid LatePaymentRuleDefinitionId { get { return new Guid("631518f4-e35f-40a8-8ac9-7d25532f7a36"); } }
        public Guid MainDataRecordStorageId { get { return new Guid("5cd31703-3bc6-41eb-b204-ef473cb394e4"); } }
        public Guid BranchTypeId { get { return new Guid("5ff96aee-cdf0-4415-a643-6b275f47e791"); } }
        public Guid CompanyTypeId { get { return new Guid("046078a0-3434-4934-8f4d-272608cffebf"); } }
        public Guid InvoiceTransactionTypeId { get { return new Guid("2B3D86AB-1689-49E8-A5FA-F65227A1EC4C"); } }
        public List<Guid> UsageTransactionTypeIds { get { return new List<Guid>() { new Guid("007869D9-6DC2-4F56-88A4-18C8C442E49E") }; } }

        public override Guid ConfigId
        {
            get { return new Guid("3C121314-56F2-445D-91C7-896D07D09BDD"); }
        }
        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(this.AccountBEDefinitionId, context.Invoice.PartnerId);

            switch (context.InfoType)
            {
                case "MailTemplate":
                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                    objects.Add("Account", financialAccountData.Account);
                    objects.Add("Invoice", context.Invoice);
                    return objects;
                case "BankDetails":
                    {
                        #region BankDetails
                        AccountBEManager accountBEManager = new AccountBEManager();
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
            return new MultiNetSubscriberInvoiceGenerator(this.AccountBEDefinitionId, this.SalesTaxChargeableEntities, this.WHTaxChargeableEntities, this.InComingChargeableEntity, this.OutGoingChargeableEntity, this.SalesTaxRuleDefinitionId, this.WHTaxRuleDefinitionId, this.LatePaymentRuleDefinitionId, this.MainDataRecordStorageId, this.BranchTypeId, this.CompanyTypeId);
        }
        public override InvoicePartnerManager GetPartnerManager()
        {
            return new MultiNetSubscriberPartnerSettings(this.AccountBEDefinitionId);
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
    }
}
