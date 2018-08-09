using System;
using System.Collections.Generic;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowActivity
    {
        public Guid VRWorkflowActivityId { get; set; }

        public VRWorkflowActivitySettings Settings { get; set; }
    }

    public class VRWorkflowActivityCollection : List<VRWorkflowActivity>
    {
    }

    public abstract class VRWorkflowActivitySettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string Editor { get; }

        public abstract string Title { get; }

        public string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine(string.Empty);
            strBuilder.AppendLine(string.Format("#region {0}", context.VRWorkflowActivityId));

            string activityCode = InternalGenerateWFActivityCode(context);
            strBuilder.AppendLine(activityCode);

            strBuilder.AppendLine("#endregion");

            return strBuilder.ToString();
        }

        protected abstract string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context);
    }

    public interface IVRWorkflowActivityGenerateWFActivityCodeContext
    {
        Guid VRWorkflowActivityId { get; set; }
        void AddVariables(VRWorkflowVariableCollection variables);

        void AddVariable(VRWorkflowVariable variable);

        VRWorkflowVariable GetVariableWithValidate(string variableName);

        IEnumerable<VRWorkflowVariable> GetAllVariables();

        IEnumerable<VRWorkflowArgument> GetAllWorkflowArguments();

        string GenerateUniqueNamespace(string nmSpace);

        void AddFullNamespaceCode(string namespaceCode);

        void AddUsingStatement(string usingStatement);

        IVRWorkflowActivityGenerateWFActivityCodeContext CreateChildContext();
    }
}
