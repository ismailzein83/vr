using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.Demo.Business
{
    public class ISPInvoiceGenerator : InvoiceGenerator
    {
        Guid _acountBEDefinitionId { get; set; }
        public ISPInvoiceGenerator(Guid acountBEDefinitionId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
        }
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
