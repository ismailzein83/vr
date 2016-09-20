using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.ExcelConversion.Entities
{
    public class ConcatenatedPartConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_ExcelConversion_ConcatenatedPart";
        public string Editor { get; set; }
    }
}

public class TestClassConfig : ExtensionConfiguration
{
    public const string EXTENSION_TYPE = "VR_TestClass_Parts";
    public string Editor { get; set; }
}
