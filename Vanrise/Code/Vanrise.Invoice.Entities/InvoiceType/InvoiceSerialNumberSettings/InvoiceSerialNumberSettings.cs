using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceSerialNumberSettings
    {
        public List<SerialNumberPart> SerialNumberParts { get; set; }
    }
    public class SerialNumberPart
    {
        public string VariableName { get; set; }
        public string Description { get; set; }
        public Vanrise.Entities.VRConcatenatedPartSettings<IInvoiceSerialNumberConcatenatedPartContext> Settings { get; set; }
       
    }
    public interface IInvoiceSerialNumberConcatenatedPartContext
    {
        Invoice Invoice { get; }
        Guid InvoiceTypeId { get; set; }
    }
}
