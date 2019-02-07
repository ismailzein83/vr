using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class SMSInvoiceSubSectionFilter : InvoiceSubSectionFilter
    {
        public override Guid ConfigId { get { return new Guid("C3240788-4078-4F24-83E4-51B36B1E42EC"); } }

        public override bool IsFilterMatch(IInvoiceSubSectionFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
