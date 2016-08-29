using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class InvoiceItemSubSection : InvoiceUISubSectionSettings
    {
        public string ItemSetName { get; set; }
        public List<InvoiceSubSectionGridColumn> GridColumns { get; set; }
    }
}
