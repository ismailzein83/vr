using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.Invoice.Entities
{
    public interface IInvoiceViewerTypeInfoFilter
    {
        bool IsExcluded(IInvoiceViewerTypeInfoFilterContext context);
    }
    public interface IInvoiceViewerTypeInfoFilterContext
    {
        Guid InvoiceViewerTypeId { get; }
    }
}
