using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowIfElseActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("40B7E3E9-F8E0-4C2C-9ED7-F79CC4A68473"); }
        }

        public string Condition { get; set; }

        public VRWorkflowActivity TrueActivity { get; set; }

        public VRWorkflowActivity FalseActivity { get; set; }

        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            //System.Activities.Statements.If wfIfActivity = new System.Activities.Statements.If(new Microsoft.CSharp.Activities.CSharpValue<bool>("1 == 1"))
            //{
            //     Then = new BaseCodeActivity(),
                  
            //};

            this.Condition.ThrowIfNull("this.Condition");
            this.TrueActivity.ThrowIfNull("this.TrueActivity");
            this.TrueActivity.Settings.ThrowIfNull("this.TrueActivity.Settings");
            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.Append(@"
            new System.Activities.Statements.If(");

            codeBuilder.Append(string.Concat("new CSharpValue<bool>(\"", this.Condition.Replace("\"", "\\\""), "\"))"));

            codeBuilder.Append(@"
            {
                Then = ");
            codeBuilder.Append(this.TrueActivity.Settings.GenerateWFActivityCode(context));

            if(this.FalseActivity != null)
            {
                this.FalseActivity.Settings.ThrowIfNull("this.FalseActivity.Settings");
                codeBuilder.Append(@",
            Else = ");
                codeBuilder.Append(this.FalseActivity.Settings.GenerateWFActivityCode(context));
            }

            codeBuilder.Append(@"
            }");

            return codeBuilder.ToString();
        }
    }
}
