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

        public override BillingPeriod GetBillingPeriod(IExtendedSettingsBillingPeriodContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {
            switch (context.InfoType)
            {
                case "MailTemplate":
                    Guid invoiceTemplateId = Guid.Empty;
                    var invoiceDetails = context.Invoice.Details as RetailSubscriberInvoiceDetails;
                    long accountId = Convert.ToInt32(context.Invoice.PartnerId);

                    AccountBEManager accountBEManager = new AccountBEManager();
                    var account = accountBEManager.GetAccount(this.AcountBEDefinitionId, accountId);
                    invoiceTemplateId = accountBEManager.GetDefaultInvoiceEmailId(accountId);

                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                    objects.Add("Account", account);
                    objects.Add("Invoice", context.Invoice);
                    VRMailManager vrMailManager = new VRMailManager();
                    VRMailEvaluatedTemplate template = vrMailManager.EvaluateMailTemplate(invoiceTemplateId, objects);
                    return template;
            }
            return null;
        }

        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new RetailSubscriberInvoiceGenerator(this.AcountBEDefinitionId);
        }

        public override InvoicePartnerSettings GetPartnerSettings()
        {
            return new RetailSubscriberPartnerSettings(this.AcountBEDefinitionId);
        }
    }
}
