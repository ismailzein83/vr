using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.CustomerAccess.Entities
{
    public abstract class InvoiceContextHandler
    {
        public abstract Guid ConfigId { get; }
        public abstract void PrepareQuery(IInvoiceContextHandlerContext context);
    }
    public interface IInvoiceContextHandlerContext
    {
        InvoiceQuery Query { get; set; }
    }
    public class InvoiceContextHandlerContext : IInvoiceContextHandlerContext
    {
        public InvoiceQuery Query { get; set; }
    }
}
