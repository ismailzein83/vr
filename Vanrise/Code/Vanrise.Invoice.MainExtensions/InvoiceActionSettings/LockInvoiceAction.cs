using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class LockInvoiceAction : InvoiceActionSettings
    {
        public override string ActionTypeName { get { return "LockInvoiceAction"; } }
        public override Guid ConfigId { get { return new Guid("82230E8D-680B-4362-8C00-D14D6E8E8AC1"); } }
        public bool SetLocked { get; set; }

        public override InvoiceActionType Type
        {
            get { return InvoiceActionType.SetInvoiceLocked; }
        }
    }
}
