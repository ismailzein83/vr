using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceTypeRuntime
    {
        public InvoiceType InvoiceType { get; set; }
        public List<InvoiceUIGridColumnRunTime> MainGridRuntimeColumns { get; set; }
    }
    public class InvoiceUIGridColumnRunTime
    {
        public string Header { get; set; }

        public InvoiceField Field { get; set; }

        public string CustomFieldName { get; set; }
        public GridColumnAttribute Attribute { get; set; }
    }
}
