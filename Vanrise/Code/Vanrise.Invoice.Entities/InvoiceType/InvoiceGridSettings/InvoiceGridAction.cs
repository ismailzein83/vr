using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceGridAction
    {
        public string Title { get; set; }

        public bool ReloadGridItem { get; set; }

        public InvoiceGridActionFilterCondition FilterCondition { get; set; }

        public Guid InvoiceGridActionId { get; set; }

    }
    public abstract class InvoiceGridActionFilterCondition
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context);
    }

    public interface IInvoiceGridActionFilterConditionContext
    {
        Invoice Invoice { get; set; }
        InvoiceType InvoiceType { get; set; }
        VRInvoiceAccount InvoiceAccount { get; }
        InvoiceAction InvoiceAction { get; set; }

    }
    public class InvoiceGridActionFilterConditionContext : IInvoiceGridActionFilterConditionContext
    {
        public Invoice Invoice { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public VRInvoiceAccount InvoiceAccount { get; set; }
        public InvoiceAction InvoiceAction { get; set; }
    }
}
