using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceTypeUISettings
    {
        public string PartnerSelector { get; set; }

        public List<InvoiceUIGridColumn> MainGridColumns { get; set; }

        public List<InvoiceUISubSection> SubSections { get; set; }

        public string GenerationCustomSectionDirective { get; set; }
    }
}
