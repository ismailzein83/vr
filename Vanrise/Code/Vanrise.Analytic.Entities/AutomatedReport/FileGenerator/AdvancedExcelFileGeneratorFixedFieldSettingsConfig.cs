using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class AdvancedExcelFileGeneratorFixedFieldSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Analytic_FileGenerator_AutomatedReport_ExcelFile_FixedField";
        public string Editor { get; set; }
    }
}
