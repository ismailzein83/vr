using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class RecreateInvoiceAction : InvoiceActionSettings
    {
        public override Guid ConfigId { get { return new Guid("6B7841D1-8ECF-48CE-88D6-440342654ADC"); } }
        public override string ActionTypeName
        {
            get { return "RecreateInvoiceAction"; }
        }

        public override InvoiceActionType Type
        {
            get { return InvoiceActionType.ReCreateInvoice; }
        }

    }
}
