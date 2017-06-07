using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public interface IInvoiceItemAdditionalFields
    {
        void FillAdditionalFields(IInvoiceItemAdditionalFieldsContext context);
    }
    public interface IInvoiceItemAdditionalFieldsContext
    {
        InvoiceType InvoiceType { get; }
    }
}
