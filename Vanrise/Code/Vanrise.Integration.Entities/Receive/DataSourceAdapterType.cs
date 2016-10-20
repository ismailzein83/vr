using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public class DataSourceAdapterType:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Integration_AdapterTypeConfig";
        public string AdapterTemplateURL { get; set; }
        public string FQTN { get; set; }
        public string Editor { get; set; }
    }
}
