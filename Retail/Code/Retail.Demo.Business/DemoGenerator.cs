
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Retail.Demo.Data;
using Retail.Demo.Entities;

namespace Retail.Demo.Business
{
    public class DemoInvoiceGenerator : InvoiceGenerator

    {
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet();
            context.Invoice = new GeneratedInvoice
            {
                InvoiceItemSets = generatedInvoiceItemSets
            };

        }

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet()
        {
            return null;
        }

    }
}
