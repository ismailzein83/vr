using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class IsSentInvoiceFilterCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("62B90539-AAAB-4A76-9967-40FEAA07A3C1"); }
        }
        public bool IsSent { get; set; }
        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            return context.Invoice.SentDate.HasValue == this.IsSent;
        }
    }
}
