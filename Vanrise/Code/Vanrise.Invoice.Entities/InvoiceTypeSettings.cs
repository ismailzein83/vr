using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceTypeSettings
    {
        public InvoiceTypeUISettings UISettings { get; set; }

        public int InvoiceDetailsRecordTypeId { get; set; }

        public InvoiceGenerator InvoiceGenerator { get; set; } 
    }   
}
