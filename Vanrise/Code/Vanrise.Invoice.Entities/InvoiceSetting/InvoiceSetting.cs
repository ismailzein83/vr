using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceSetting
    {
        public Guid InvoiceSettingId { get; set; }
        public string Name { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public bool IsDefault { get; set; }
        public InvoiceSettingDetails Details { get; set; }
    }
    public class InvoiceSettingDetails
    {
        public Dictionary<Guid, InvoiceSettingPart> InvoiceSettingParts { get; set; }
    }
    public abstract class InvoiceSettingPart
    {
        public abstract  Guid ConfigId { get;}

    }
}
