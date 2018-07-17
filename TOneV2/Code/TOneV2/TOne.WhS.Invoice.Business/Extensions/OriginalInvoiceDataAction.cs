using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.Invoice.Business.Extensions
{public enum InvoiceCarrierType { Customer = 0, Supplier = 1 };
    public class OriginalInvoiceDataAction : InvoiceActionSettings
    {
        public override string ActionTypeName { get { return "OriginalInvoiceData"; } }
        public override Guid ConfigId { get { return new Guid("CA9DF16B-B492-4C86-9FE5-602A990E4D24"); } }
        public InvoiceCarrierType InvoiceCarrierType { get; set; }
        public override bool DoesUserHaveAccess(IInvoiceActionSettingsCheckAccessContext context)
        {
            return SecurityContext.Current.IsAllowed(context.InvoiceAction.RequiredPermission, context.UserId);
        }
    }
}
