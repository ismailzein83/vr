using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowVariableTypeConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "BusinessProcess_VRWorkflowVariableTypeConfig";
        public string Description { get; set; }
        public string Editor { get; set; }
    }

	public class VRWorkflowActivityConfig : ExtensionConfiguration
	{
		public const string EXTENSION_TYPE = "BP_VR_Workflow_Activity";
		public string Editor { get; set; }
	}
}
