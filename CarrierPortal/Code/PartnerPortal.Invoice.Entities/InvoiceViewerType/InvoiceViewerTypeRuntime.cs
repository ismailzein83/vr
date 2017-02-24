using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.Invoice.Entities
{
    public class InvoiceViewerTypeRuntime
    {
        public List<RuntimeGridColumn> RuntimeGridColumns { get; set; }
        public List<InvoiceViewerTypeGridAction> InvoiceGridActions { get; set; }
    }
    public class RuntimeGridColumn
    {
        public string Header { get; set; }

        public InvoiceField Field { get; set; }
        public string FieldName { get; set; }
        public string CustomFieldName { get; set; }
        public GridColumnAttribute Attribute { get; set; }
    }
}
