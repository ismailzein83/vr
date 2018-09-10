using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.BP.Arguments
{
    public class InvoiceGenerationProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            var invoiceTypeManager = BusinessManagerFactory.GetManager<IInvoiceTypeManager>();
            return string.Format("Invoice Generation Process - {0}", invoiceTypeManager.GetInvoiceTypeName(InvoiceTypeId));
        }

        public Guid InvoiceTypeId { get; set; }
        public Guid InvoiceGenerationIdentifier { get; set; }
        public bool IsAutomatic { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime MinimumFrom { get; set; }
        public DateTime MaximumTo { get; set; }
        public InvoiceGapAction InvoiceGapAction { get; set; }

    }
}
