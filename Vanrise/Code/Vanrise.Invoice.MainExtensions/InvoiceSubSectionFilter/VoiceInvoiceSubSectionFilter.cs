using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class VoiceInvoiceSubSectionFilter : InvoiceSubSectionFilter
    {
        public override Guid ConfigId { get { return new Guid("8D540CD4-1618-437A-B713-441DD32A671A"); } }

        public override bool IsFilterMatch(IInvoiceSubSectionFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
