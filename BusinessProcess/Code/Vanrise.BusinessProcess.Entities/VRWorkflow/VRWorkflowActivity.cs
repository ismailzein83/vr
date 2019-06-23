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

        public bool IsDisabled { get; set; }

        public string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            if (this.IsDisabled)
                return string.Empty;

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine(string.Empty);
            strBuilder.AppendLine(string.Format("#region {0}", context.VRWorkflowActivityId));

            string activityCode = InternalGenerateWFActivityCode(context);
            strBuilder.AppendLine(activityCode);

            strBuilder.AppendLine("#endregion");

            return strBuilder.ToString();
        }

        protected abstract string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context);

        public virtual BPVisualItemDefinition GetVisualItemDefinition(IVRWorkflowActivityGetVisualItemDefinitionContext context)
        {
            return null;
        }
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

        string GenerateInsertVisualEventCode(GenerateInsertVisualEventCodeInput input);

        IVRWorkflowActivityGenerateWFActivityCodeContext CreateChildContext(Func<GenerateInsertVisualEventCodeInput, string> generateInsertVisualEventCodeAction);
    }

    public class GenerateInsertVisualEventCodeInput
    {
        public string ActivityContextVariableName { get; set; }

        public Guid ActivityId { get; set; }

        public string EventTitle { get; set; }

        public Guid EventTypeId { get; set; }

        public string EventPayloadCode { get; set; }
    }

    public interface IVRWorkflowActivityGetVisualItemDefinitionContext
    {
        string SubProcessActivityName { get; }
    }
}
