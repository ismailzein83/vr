using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace TOne.WhS.Invoice.Entities
{
   public class InvoiceComparisonTemplate
    {
        public long InvoiceComparisonTemplateId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public InvoiceComparisonTemplateDetails Details { get; set; }
    }
    public class InvoiceComparisonTemplateDetails
    {
        public ListMapping ListMapping { get; set; }
        public string DateTimeFormat { get; set; }
    }
}
