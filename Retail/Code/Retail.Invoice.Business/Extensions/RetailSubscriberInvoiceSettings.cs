using Retail.BusinessEntity.Business;
using Retail.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.Invoice.Business
{
    public class RetailSubscriberInvoiceSettings : InvoiceTypeExtendedSettings
    {
        public Guid AcountBEDefinitionId { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("2f5c2fb4-4380-4a18-986c-210459134b4b"); }
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
            return new RetailSubscriberInvoiceGenerator(this.AcountBEDefinitionId);
        }

        public override InvoicePartnerManager GetPartnerManager()
        {
            return new RetailSubscriberPartnerSettings(this.AcountBEDefinitionId);
        }

        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            var financialAccounts = accountBEManager.GetFinancialAccounts(this.AcountBEDefinitionId);
            if (financialAccounts == null)
                return null;
            return financialAccounts.Select(x => x.AccountId.ToString());
        }
    }
}
