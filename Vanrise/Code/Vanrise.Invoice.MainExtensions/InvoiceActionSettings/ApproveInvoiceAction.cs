using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class ApproveInvoiceAction : InvoiceActionSettings
    {
        public override Guid ConfigId
        {
            get
            {
                return new Guid("F2DBA69C-B86B-48DB-A49B-A30A72C50D1E");
            }
        }
        public override InvoiceActionType Type
        {
            get { return InvoiceActionType.Approve; }
        }
        public override string ActionTypeName
        {
            get { return "ApproveInvoiceAction"; }
        }
        public override bool DoesUserHaveAccess(IInvoiceActionSettingsCheckAccessContext context)
        {
            return ContextFactory.GetContext().IsAllowed(context.InvoiceAction.RequiredPermission, context.UserId);
        }
    }
}
