using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class SetInvoiceDeletedAction : InvoiceActionSettings
    {
        public override Guid ConfigId { get { return new Guid("EBF6D7EB-EEC7-44B9-B1D4-F4A435680C14"); } }
        public override string ActionTypeName
        {
            get { return "SetInvoiceDeletedAction"; }
        }
        public override InvoiceActionType Type
        {
            get { return InvoiceActionType.SetInvoiceDeleted; }
        }
       
    }
}
