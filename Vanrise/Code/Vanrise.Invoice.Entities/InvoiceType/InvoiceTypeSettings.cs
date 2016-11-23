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
        public Guid InvoiceDetailsRecordTypeId { get; set; }
        public List<InvoiceAction> InvoiceActions { get; set; }
        public List<InvoiceGeneratorAction> InvoiceGeneratorActions { get; set; }
        public InvoiceTypeExtendedSettings ExtendedSettings { get; set; }
        public InvoiceGridSettings InvoiceGridSettings { get; set; }
        public InvoiceSerialNumberSettings InvoiceSerialNumberSettings { get; set; }
        public List<InvoiceSubSection> SubSections { get; set; }
    }
}
