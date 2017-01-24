using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class PartnerInvoiceFilterCondition
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsFilterMatch(IPartnerInvoiceFilterConditionContext context);
    }

    public interface IPartnerInvoiceFilterConditionContext
    {
        InvoiceType InvoiceType { get; }
        GenerateInvoiceInput generateInvoiceInput { get; }
    }
    public class PartnerInvoiceFilterConditionContext : IPartnerInvoiceFilterConditionContext
    {
        public InvoiceType InvoiceType { get; set; }
        public GenerateInvoiceInput generateInvoiceInput { get; set; }
    }
}
