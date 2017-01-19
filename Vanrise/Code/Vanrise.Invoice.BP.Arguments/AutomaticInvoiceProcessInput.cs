using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.BP.Arguments
{
    public class AutomaticInvoiceProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid InvoiceTypeId { get; set; }
        public override string GetTitle()
        {
            return "Automatic Invoice Process";
        }
    }
}
