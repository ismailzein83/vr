using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceGridAction
    {
        public string Title { get; set; }
        public bool ReloadGridItem { get; set; }
        public InvoiceFilterCondition InvoiceFilterCondition { get; set; }
        public InvoiceGridActionSettings Settings { get; set; }
        
    }
    public abstract class InvoiceFilterCondition
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsFilterMatch(IInvoiceFilterConditionContext context);
    }
    public interface IInvoiceFilterConditionContext
    {
        Invoice Invoice { get; set; }
        InvoiceType InvoiceType { get; set; }
    }
    public class InvoiceFilterConditionContext : IInvoiceFilterConditionContext
    {
        public Invoice Invoice { get; set;}
        public InvoiceType InvoiceType { get; set; }
    }
}
