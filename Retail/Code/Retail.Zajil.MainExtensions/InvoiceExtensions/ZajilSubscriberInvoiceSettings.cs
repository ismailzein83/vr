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
    public class ZajilSubscriberInvoiceSettings : InvoiceTypeExtendedSettings
    {
        public Guid AcountBEDefinitionId { get; set; }
        public Guid CompanyExtendedInfoPartdefinitionId { get; set; }

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
                    var account = accountBEManager.GetAccount(this.AcountBEDefinitionId, accountId);
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
            var account = accountBEManager.GetAccount(this.AcountBEDefinitionId, accountId);
            context.PartnerCreationDate = account.CreatedTime;

        }

        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new ZajilSubscriberInvoiceGenerator(this.AcountBEDefinitionId, this.CompanyExtendedInfoPartdefinitionId);
        }

        public override InvoicePartnerManager GetPartnerManager()
        {
            return new ZajilSubscriberPartnerSettings(this.AcountBEDefinitionId);
        }

        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            switch (context.PartnerRetrievalType)
            {
                case PartnerRetrievalType.GetActive:
                case PartnerRetrievalType.GetAll:
                    AccountBEManager accountBEManager = new AccountBEManager();
                    var financialAccounts = accountBEManager.GetFinancialAccounts(this.AcountBEDefinitionId);
                    if (financialAccounts == null)
                        return null;
                    return financialAccounts.Select(x => x.AccountId.ToString());
                default:
                    return null;
            }

        }
    }
}
