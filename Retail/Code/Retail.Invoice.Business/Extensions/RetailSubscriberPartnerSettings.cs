using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.Invoice.Business
{
    public class RetailSubscriberPartnerSettings : InvoicePartnerSettings
    {
        public override string PartnerFilterSelector
        {
            get
            {
                return "retail-invoice-financialaccount-selector";
            }
        }

        public override string PartnerSelector
        {
            get
            {
                return "retail-invoice-financialaccount-selector";
            }
        }

        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            return Convert.ToInt32(context.PartnerId);
        }

        public override int GetPartnerDuePeriod(IPartnerDuePeriodContext context)
        {
            AccountManager accountManager = new AccountManager();
            return accountManager.GetAccountDuePeriod(Convert.ToInt32(context.PartnerId));
        }

        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            AccountManager accountManager = new AccountManager();
            return accountManager.GetAccountName(Convert.ToInt32(context.PartnerId));
        }
    }
}
