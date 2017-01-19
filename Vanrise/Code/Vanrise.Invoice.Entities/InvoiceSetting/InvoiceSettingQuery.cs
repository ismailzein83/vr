using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceSettingQuery
    {
        public string Name { get; set; }
        public Guid InvoiceTypeId { get; set; }
    }
}
