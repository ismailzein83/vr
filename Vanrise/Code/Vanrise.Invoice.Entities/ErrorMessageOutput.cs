using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
namespace Vanrise.Invoice.Entities
{
    public class InvoiceGenerationMessageOutput
    {
        public string Message { get; set; }
        public LogEntryType LogEntryType { get; set; }
    }
}
