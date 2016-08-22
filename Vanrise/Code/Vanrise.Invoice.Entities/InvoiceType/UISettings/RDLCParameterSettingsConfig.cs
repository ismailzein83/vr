using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class RDLCParameterSettingsConfig:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Invoice_InvoiceType_DLCParameterSettings";
        public string Editor { get; set; }
    }
}
