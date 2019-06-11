using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericEditorConditionalRulesConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_GenericEditorConditionalRulesConfig";

        public string Editor { get; set; }
    }
}