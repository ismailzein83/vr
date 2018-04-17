using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceAction
    {
        public Guid InvoiceActionId { get; set; }

        public string Title { get; set; }
        public InvoiceActionSettings Settings { get; set; }

        public RequiredPermissionSettings RequiredPermission { get; set; }

    }
    public abstract class InvoiceActionSettings
    {
        public virtual string ActionTypeName { get; set; }
        public virtual InvoiceActionType Type { get; set; }
        public abstract Guid ConfigId { get; }
        public virtual bool DoesUserHaveAccess(IInvoiceActionSettingsCheckAccessContext context)
        {

            return ContextFactory.GetContext().IsAllowed(context.InvoiceAction.RequiredPermission , context.UserId);
        }
    }

    public enum InvoiceActionType
    {
        ReCreateInvoice = 0,
        SendMail = 1,
        SetInvoicePaid = 2,
        UpdateInvoiceNote = 3,
        SetInvoiceLocked = 4,
        SetInvoiceDeleted = 5,
        Download = 6,
        CustomActionType = 7,
        Approve = 8
    }

    public interface IInvoiceActionSettingsCheckAccessContext
    {
        int UserId { get; }
        InvoiceAction InvoiceAction { get; set; }

    }

    public class InvoiceActionSettingsCheckAccessContext : IInvoiceActionSettingsCheckAccessContext
    {
        public int UserId { get; set; }
        public InvoiceAction InvoiceAction { get; set; }

    }
}
