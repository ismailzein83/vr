using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class IsPaidInvoiceFilterCondition : InvoiceFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("20359B38-3A22-4D31-ACAA-CEB099D5A62C"); }
        }
        public bool IsPaid { get; set; }
        public override bool IsFilterMatch(IInvoiceFilterConditionContext context)
        {
            return context.Invoice.PaidDate.HasValue == this.IsPaid;
        }
    }
}
