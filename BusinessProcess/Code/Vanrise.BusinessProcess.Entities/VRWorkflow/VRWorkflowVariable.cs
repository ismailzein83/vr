using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowVariable
    {
        public Guid VRWorkflowVariableId { get; set; }

        public string Name { get; set; }

        public VRWorkflowVariableType Type { get; set; }
    }

    public class VRWorkflowVariableCollection : List<VRWorkflowVariable>
    {
        public string GenerateVariablesCode()
        {
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.Append("{");
            codeBuilder.AppendLine();

            bool isFirstVariable = true;
            foreach (var vrVariable in this)
            {
                if (!isFirstVariable)
                    codeBuilder.Append(",");
                else
                    isFirstVariable = false;
                vrVariable.Name.ThrowIfNull("vrVariable.Name");
                vrVariable.Type.ThrowIfNull("vrVariable.Type", vrVariable.Name);
                Type variableRuntimeType = vrVariable.Type.GetRuntimeType(null);
                codeBuilder.AppendFormat(@" new Variable<{0}>(""{1}"")", CSharpCompiler.TypeToString(variableRuntimeType), vrVariable.Name);
                codeBuilder.AppendLine();
            }
            codeBuilder.AppendLine();
            codeBuilder.Append("}");
            codeBuilder.AppendLine();
            return codeBuilder.ToString();
        }
    }
}
