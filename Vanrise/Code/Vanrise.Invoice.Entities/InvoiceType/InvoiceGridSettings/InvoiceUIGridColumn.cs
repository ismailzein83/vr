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
    }
}
