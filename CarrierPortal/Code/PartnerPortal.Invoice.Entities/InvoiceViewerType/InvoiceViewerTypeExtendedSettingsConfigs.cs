using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PartnerPortal.Invoice.Entities
{
    public class InvoiceViewerTypeExtendedSettingsConfigs : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "PartnerPortal_Invoice_InvoiceViewerTypeExtendedSettings";
        public string Editor { get; set; }
    }
}
