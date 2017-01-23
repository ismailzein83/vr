using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceSettingPartConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Invoice_InvoiceSettingPartConfig";
        public string DefinitionEditor { get; set; }
        public string RuntimeEditor { get; set; }
    }
}
