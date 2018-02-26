using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class SetInvoicePaidAction : InvoiceActionSettings
    {
        public override Guid ConfigId { get { return new Guid("F974FC02-9C97-4C04-9DD3-2501A3807BFE"); } }
        public override string ActionTypeName
        {
            get { return "SetInvoicePaidAction"; }
        }

        public override InvoiceActionType Type
        {
            get { return InvoiceActionType.SetInvoicePaid; }
        }
        public bool IsInvoicePaid { get; set; }
    }
}
