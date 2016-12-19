using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class ItemGroupingSectionResult : GroupingInvoiceItemDetail
    {
        public List<Guid> SubSectionsIds { get; set; }
    }
}
