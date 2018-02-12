using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericBEOnAfterSaveHandlerSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_ GenericBEOnAfterSaveHandlerSettings";
        public string Editor { get; set; }
    }
}
