using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class RDBDataRecordStorageExpressionFieldSettingsConfig: ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_RDBDataRecordStorageExpressionFieldSettingsConfig";
        public string Editor { get; set; }
    }
}
