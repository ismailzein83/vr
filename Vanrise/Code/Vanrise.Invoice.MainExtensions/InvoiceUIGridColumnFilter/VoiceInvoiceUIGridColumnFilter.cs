using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class VoiceInvoiceUIGridColumnFilter : InvoiceUIGridColumnFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("0fa2e4e4-3468-4a32-bedc-2c302300ae6b"); }
        }

        public override bool IsFilterMatch(IInvoiceUIGridColumnFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
