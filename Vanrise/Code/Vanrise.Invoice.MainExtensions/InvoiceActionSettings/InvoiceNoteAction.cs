using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class InvoiceNoteAction : InvoiceActionSettings
    {
        public override string ActionTypeName { get { return "InvoiceNoteAction"; } }
        public override Guid ConfigId { get { return new Guid("56DB2DBA-3A2C-4D62-A782-5BE05B35EF46"); } }

        public override InvoiceActionType Type
        {
            get { return InvoiceActionType.UpdateInvoiceNote; }
        }
    }
}
