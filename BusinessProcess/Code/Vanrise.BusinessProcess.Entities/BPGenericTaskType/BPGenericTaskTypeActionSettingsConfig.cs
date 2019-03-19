using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPGenericTaskTypeActionSettingsConfig: ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "BP_Generic_TaskType_ActionSettings";

        public string Editor { get; set; }
    }
}
