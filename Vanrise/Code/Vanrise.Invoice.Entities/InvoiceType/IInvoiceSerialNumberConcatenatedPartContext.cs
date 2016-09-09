using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public interface IInvoiceSerialNumberConcatenatedPartContext
    {
         Invoice Invoice { get;}
         Guid InvoiceTypeId { get; set; }
    }
}
