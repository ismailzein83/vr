using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class IsLockedGridFilterCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("B220DD18-D38E-4170-8E80-7508854C29F6"); }
        }
        public bool IsLocked { get; set; }
        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            return context.Invoice.LockDate.HasValue == this.IsLocked;
        }
    }
}
