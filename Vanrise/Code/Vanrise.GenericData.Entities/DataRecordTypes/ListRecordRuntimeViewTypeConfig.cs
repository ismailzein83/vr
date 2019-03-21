using System;
using Vanrise.Entities;
namespace Vanrise.GenericData.Entities
{
    public class ListRecordRuntimeViewTypeConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_ListRecordRuntimeViewType";
        public string Editor { get; set; }
        public string RuntimeEditor { get; set; }
        public string FilterEditor { get; set; }
        public bool IsSupportedInGenericRule { get; set; }
        public string RuleFilterEditor { get; set; }
    }
}
