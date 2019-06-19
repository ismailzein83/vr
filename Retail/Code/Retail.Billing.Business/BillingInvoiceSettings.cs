using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.MainExtensions;

namespace Retail.Billing.Business
{
    public class BillingInvoiceSettings : GenericInvoiceSettings
    {
        public override Guid ConfigId { get { return new Guid("1535DBF0-01E9-4E8B-9566-A278F357CC80"); } }

        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {
            return null;
        }

        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
        }

        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return null;
        }

        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            return null;
        }

        public override InvoicePartnerManager GetPartnerManager()
        {
            return null;
        }
    }
}
