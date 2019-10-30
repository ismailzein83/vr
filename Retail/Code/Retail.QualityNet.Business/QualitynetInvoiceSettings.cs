using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using Vanrise.Invoice.Entities;

namespace Retail.QualityNet.Business
{
    public class QualityNetInvoiceTypeSettings : BaseRetailInvoiceTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("D8AF155C-4303-491B-B8DD-553DEEEB9C68"); } }
        public Guid MainDataRecordStorageId { get { return new Guid("5cd31703-3bc6-41eb-b204-ef473cb394e4"); } }
        public Guid InvoiceTransactionTypeId { get { return new Guid("2B3D86AB-1689-49E8-A5FA-F65227A1EC4C"); } }
        public List<Guid> UsageTransactionTypeIds { get { return new List<Guid>() { new Guid("007869D9-6DC2-4F56-88A4-18C8C442E49E") }; } }

        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            long accountId = Convert.ToInt64(context.Invoice.PartnerId);

            switch (context.InfoType)
            {
                case "MailTemplate":
                    var account = accountBEManager.GetAccount(this.AccountBEDefinitionId, accountId);
                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                    objects.Add("Operator", account);
                    objects.Add("Invoice", context.Invoice);
                    return objects;

                case "BankDetails":
                    return accountBEManager.GetBankDetailsIds(this.AccountBEDefinitionId, accountId);
            }
            return null;
        }

        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            long accountId = Convert.ToInt64(context.PartnerId);
            var account = new AccountBEManager().GetAccount(this.AccountBEDefinitionId, accountId);
            context.PartnerCreationDate = account.CreatedTime;
        }

        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new QualityNetInvoiceGenerator(this.AccountBEDefinitionId, this.MainDataRecordStorageId);
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
            return new QualityNetInvoicePartnerSettings(this.AccountBEDefinitionId);
        }
    }
}