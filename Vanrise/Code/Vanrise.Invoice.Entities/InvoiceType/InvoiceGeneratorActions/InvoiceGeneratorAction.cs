using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceGeneratorAction
    {
        public string Title { get; set; }
        public VRButtonType ButtonType { get; set; }
        public Guid InvoiceGeneratorActionId { get; set; }
        public InvoiceGeneratorActionFilterCondition FilterCondition { get; set; }

    }

    public abstract class InvoiceGeneratorActionFilterCondition
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsFilterMatch(IInvoiceGeneratorActionFilterConditionContext context);
    }

    public interface IInvoiceGeneratorActionFilterConditionContext
    {
        InvoiceType InvoiceType { get;}
        GenerateInvoiceInput generateInvoiceInput { get;}
    }
    public class InvoiceGeneratorActionFilterConditionContext : IInvoiceGeneratorActionFilterConditionContext
    {
        public InvoiceType InvoiceType { get; set; }
        public GenerateInvoiceInput generateInvoiceInput { get; set; }
    }
}
