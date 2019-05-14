using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.RDBDataStorage
{
    public class RDBDataRecordStorageJoinSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_RDBDataRecordStorageJoinSettingsConfig";
        public string Editor { get; set; }
    }
}
