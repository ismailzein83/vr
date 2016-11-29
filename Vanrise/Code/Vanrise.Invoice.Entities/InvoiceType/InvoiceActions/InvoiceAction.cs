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
        public abstract Guid ConfigId { get; }
    }
}
