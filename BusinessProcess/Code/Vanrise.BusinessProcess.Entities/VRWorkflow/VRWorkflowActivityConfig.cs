using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowActivityConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "BP_VR_Workflow_Activity";

        public string Editor { get; set; }
    }

    public abstract class VRWorkflowAddBEActivitySettings
    {
        public abstract string GenerateCode(IVRWorkflowAddBEActivitySettingsGenerateCodeContext context);
    }

    public interface IVRWorkflowAddBEActivitySettingsGenerateCodeContext
    {
        Guid EntityDefinitionId { get; }
    }

    public abstract class VRWorkflowUpdateBEActivitySettings
    {
        public abstract string GenerateCode(IVRWorkflowUpdateBEActivitySettingsGenerateCodeContext context);
    }

    public interface IVRWorkflowUpdateBEActivitySettingsGenerateCodeContext
    {
        Guid EntityDefinitionId { get; }

        string EntityIdCode { get; }
    }
}
