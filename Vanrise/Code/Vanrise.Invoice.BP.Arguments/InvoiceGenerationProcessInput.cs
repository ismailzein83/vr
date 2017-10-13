using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.BP.Arguments
{
    public class InvoiceGenerationProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Invoice Generation Process";
        }

        public Guid InvoiceTypeId { get; set; }
        public Guid InvoiceGenerationIdentifier { get; set; }
    }
}
