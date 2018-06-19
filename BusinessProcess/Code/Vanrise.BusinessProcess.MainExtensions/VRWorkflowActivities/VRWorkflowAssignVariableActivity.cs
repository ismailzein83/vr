using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowAssignVariableActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public string Variable { get; set; }

        public string Value { get; set; }

        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            this.Variable.ThrowIfNull("this.Variable");
            this.Value.ThrowIfNull("this.Value");
            VRWorkflowVariable variable = context.GetVariableWithValidate(this.Variable);
            string runtimeType = CSharpCompiler.TypeToString(variable.Type.GetRuntimeType(null));
            return string.Concat("new Assign { To = new OutArgument<", runtimeType, ">(new CSharpReference<", runtimeType,
                ">(\"", this.Variable, "\")), Value = new InArgument<", runtimeType, ">(new CSharpValue<", runtimeType, ">(\"", this.Value.Replace("\"", "\\\""), "\"))  }");
        }
    }
}
