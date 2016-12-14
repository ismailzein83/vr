using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.Invoice.Business
{
    public class RetailSubscriberInvoiceSettings : InvoiceTypeExtendedSettings
    {

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
            throw new NotImplementedException();
        }

        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new RetailSubscriberInvoiceGenerator();
        }

        public override InvoicePartnerSettings GetPartnerSettings()
        {
            return new RetailSubscriberPartnerSettings();
        }
    }
}
