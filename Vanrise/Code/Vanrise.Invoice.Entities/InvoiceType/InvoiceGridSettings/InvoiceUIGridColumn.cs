using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceUIGridColumn
    {
        public string Header { get; set; }

        public InvoiceField Field { get; set; }

        public string CustomFieldName { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }
        public bool UseDescription { get; set; }
        public InvoiceUIGridColumnFilter Filter { get; set; }
    }
    public abstract class InvoiceUIGridColumnFilter
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsFilterMatch(IInvoiceUIGridColumnFilterContext context);

    }
    public interface IInvoiceUIGridColumnFilterContext
    {
        InvoiceType InvoiceType { get; }
    }
    public class InvoiceUIGridColumnFilterContext : IInvoiceUIGridColumnFilterContext
    {
        public InvoiceType InvoiceType { get; set; }
    }
}
