using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public interface IInvoicePartnerFilter
    {
        bool IsMatched(IInvoicePartnerFilterContext context);
    }
    public interface IInvoicePartnerFilterContext
    {
       // Guid InvoiceSettingId { get; }
        string PartnerId { get; }
    }
}
