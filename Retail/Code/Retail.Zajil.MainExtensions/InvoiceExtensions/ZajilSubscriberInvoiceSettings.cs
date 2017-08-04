using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.Zajil.MainExtensions
{
    public class ZajilSubscriberInvoiceSettings : BaseRetailInvoiceTypeSettings
    {        
        public Guid CompanyExtendedInfoPartdefinitionId { get; set; }
        public Guid InvoiceTransactionTypeId { get { return new Guid("2B3D86AB-1689-49E8-A5FA-F65227A1EC4C"); } }
        public List<Guid> UsageTransactionTypeIds { get { return new List<Guid>() { new Guid("007869D9-6DC2-4F56-88A4-18C8C442E49E") }; } }

        public override Guid ConfigId
        {
            get { return new Guid("F0A5A4EC-B1D9-4F4E-957D-7A055469DAF8"); }
        }

        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {
            switch (context.InfoType)
            {
                case "MailTemplate":
                    long accountId = Convert.ToInt32(context.Invoice.PartnerId);
                    AccountBEManager accountBEManager = new AccountBEManager();
                    var account = accountBEManager.GetAccount(this.AccountBEDefinitionId, accountId);
                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                    objects.Add("Account", account);
                    objects.Add("Invoice", context.Invoice);
                    return objects;
            }
            return null;
        }

        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            long accountId = Convert.ToInt32(context.PartnerId);
            AccountBEManager accountBEManager = new AccountBEManager();
            var account = accountBEManager.GetAccount(this.AccountBEDefinitionId, accountId);
            context.PartnerCreationDate = account.CreatedTime;

        }

        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new ZajilSubscriberInvoiceGenerator(this.AccountBEDefinitionId, this.CompanyExtendedInfoPartdefinitionId, this.InvoiceTransactionTypeId,this.UsageTransactionTypeIds);
        }

        public override InvoicePartnerManager GetPartnerManager()
        {
            return new ZajilSubscriberPartnerSettings(this.AccountBEDefinitionId);
        }

        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            switch (context.PartnerRetrievalType)
            {
                case PartnerRetrievalType.GetActive:
                case PartnerRetrievalType.GetAll:
                    AccountBEManager accountBEManager = new AccountBEManager();
                    var financialAccounts = accountBEManager.GetFinancialAccounts(this.AccountBEDefinitionId);
                    if (financialAccounts == null)
                        return null;
                    return financialAccounts.Select(x => x.AccountId.ToString());
                default:
                    return null;
            }

        }
    }
}
