using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.Invoice.Entities
{
    public abstract class InvoiceQueryInterceptor 
    {
        public abstract Guid ConfigId { get; }
        public abstract void PrepareQuery(IInvoiceQueryInterceptorContext context);
    }
    public interface IInvoiceQueryInterceptorContext
    {
        InvoiceQuery Query { get; set; }
    }
    public class InvoiceQueryInterceptorContext : IInvoiceQueryInterceptorContext
    {
        public InvoiceQuery Query { get; set; }
    }
}
