using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class CriteriaDefinitionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_CriteriaDefinition";
        public string GridEditor { get; set; }
        public string SearchEditor { get; set; }
        public string DefinitionEditor { get; set; }
    }
}