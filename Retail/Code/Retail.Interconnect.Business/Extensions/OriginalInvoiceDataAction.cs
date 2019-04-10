using Retail.Interconnect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;

namespace Retail.Interconnect.Business.Extensions
{
    public class OriginalInvoiceDataAction : InvoiceActionSettings
    {
        public override string ActionTypeName { get { return "OriginalInvoiceData"; } }
        public override Guid ConfigId { get { return new Guid("C2397DE0-4DC5-4EE0-BAC1-D1CFBE9C2D0D"); } }
        public InvoiceCarrierType InvoiceCarrierType { get; set; }
        public override bool DoesUserHaveAccess(IInvoiceActionSettingsCheckAccessContext context)
        {
            return SecurityContext.Current.IsAllowed(context.InvoiceAction.RequiredPermission, context.UserId);
        }
    }
}
