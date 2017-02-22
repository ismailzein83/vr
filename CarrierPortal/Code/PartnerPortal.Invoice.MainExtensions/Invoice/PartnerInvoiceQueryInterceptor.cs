using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Business;

namespace PartnerPortal.Invoice.MainExtensions.Invoice
{
    public class PartnerInvoiceQueryInterceptor : InvoiceQueryInterceptor
    {
        public override Guid ConfigId
        {
            get {return new Guid("12371BE0-CF2C-4CDD-9F4C-E809D912A716"); }
        }

        public override void PrepareQuery(IInvoiceQueryInterceptorContext context)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            context.Query.PartnerIds = new List<string>();
            context.Query.PartnerIds.Add(manager.GetRetailAccountId(userId).ToString());
        }
    }
}
