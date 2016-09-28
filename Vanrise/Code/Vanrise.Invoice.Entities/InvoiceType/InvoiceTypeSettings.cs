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

        public Guid InvoiceDetailsRecordTypeId { get; set; }

        public InvoiceGenerator InvoiceGenerator { get; set; }
        public List<SerialNumberPart> SerialNumberParts { get; set; }
        public string SerialNumberPattern { get; set; }
    }
    public class SerialNumberPart
    {
        public string VariableName { get; set; }
        public string Description { get; set; }
        public Vanrise.Entities.VRConcatenatedPartSettings<IInvoiceSerialNumberConcatenatedPartContext> Settings { get; set; }
    }
}
