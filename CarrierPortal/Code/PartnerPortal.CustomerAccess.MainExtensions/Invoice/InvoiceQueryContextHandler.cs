using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Business;

namespace PartnerPortal.CustomerAccess.MainExtensions.Invoice
{
    public class InvoiceQueryContextHandler : InvoiceContextHandler
    {
        public override Guid ConfigId
        {
            get {return new Guid("12371BE0-CF2C-4CDD-9F4C-E809D912A716"); }
        }

        public override void PrepareQuery(IInvoiceContextHandlerContext context)
        {
            //InvoiceContextHandlerContext invoiceContextHandlerContext = context as InvoiceContextHandlerContext;
            //if (invoiceContextHandlerContext == null)
            //    throw new Exception("invoiceContextHandlerContext is not of type InvoiceContextHandlerContext.");
            //int userId = SecurityContext.Current.GetLoggedInUserId();
            //RetailAccountUserManager manager = new RetailAccountUserManager();
            //invoiceContextHandlerContext.Query.AccountId = manager.GetRetailAccountId(userId).ToString();
        }
    }
}
