using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowVariableTypeConfig:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "BusinessProcess_VRWorkflowVariableTypeConfig";
        public string Description { get; set; }
        public string Editor { get; set; }
    }
}
