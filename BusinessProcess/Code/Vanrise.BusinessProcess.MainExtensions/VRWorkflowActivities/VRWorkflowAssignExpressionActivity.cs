using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowAssignExpressionActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public string To { get; set; }

        public string Value { get; set; }

        public string ExpressionRuntimeType { get; set; }

        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            this.To.ThrowIfNull("this.To");
            this.Value.ThrowIfNull("this.Value");
            this.ExpressionRuntimeType.ThrowIfNull("this.ExpressionRuntimeType");
            
            return string.Concat("new Assign { To = new OutArgument<", this.ExpressionRuntimeType, ">(new CSharpReference<", this.ExpressionRuntimeType,
               ">(\"", this.To, "\")), Value = new InArgument<", this.ExpressionRuntimeType, ">(new CSharpValue<", this.ExpressionRuntimeType, ">(\"", this.Value.Replace("\"", "\\\""), "\"))  }");
        }
    }
}
