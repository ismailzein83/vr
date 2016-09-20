using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.ExcelConversion.Entities
{
    public class FieldMappingConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_ExcelConversion_FieldMapping";
        public string Editor { get; set; }
    }
}
