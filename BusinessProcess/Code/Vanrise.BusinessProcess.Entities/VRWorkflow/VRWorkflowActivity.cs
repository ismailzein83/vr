using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowActivity
    {
        public Guid VRWorkflowActivityId { get; set; }

        public VRWorkflowActivitySettings Settings { get; set; }
    }

    public abstract class VRWorkflowActivitySettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context);
    }

    public interface IVRWorkflowActivityGenerateWFActivityCodeContext
    {
        void AddVariables(VRWorkflowVariableCollection variables);

        VRWorkflowVariable GetVariableWithValidate(string variableName);

        IEnumerable<VRWorkflowVariable> GetAllVariables();
        
        IEnumerable<VRWorkflowArgument> GetAllWorkflowArguments();

        string GenerateUniqueNamespace(string nmSpace);

        void AddFullNamespaceCode(string namespaceCode);

        void AddUsingStatement(string usingStatement);
    }
}
