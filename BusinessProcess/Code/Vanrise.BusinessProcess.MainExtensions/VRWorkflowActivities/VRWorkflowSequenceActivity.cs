using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowSequenceActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9292B3BE-256F-400F-9BC6-A0423FA0B30F"); }
        }

        public List<VRWorkflowActivity> Activities { get; set; }

        public VRWorkflowVariableCollection Variables { get; set; }
        
        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.Append(@"
            new Sequence
            {");
            codeBuilder.AppendLine();

            if (this.Variables != null)
            {
                context.AddVariables(this.Variables);
                codeBuilder.Append(@"
                Variables = ");
                codeBuilder.Append(this.Variables.GenerateVariablesCode());
            }

            if (this.Activities != null)
            {
                if (this.Variables != null)
                    codeBuilder.Append(",");
                codeBuilder.Append(@"
                Activities = 
                {");
                codeBuilder.AppendLine();

                bool isFirstActivity = true;
                foreach (var vrActivity in this.Activities)
                {
                    if (!isFirstActivity)
                        codeBuilder.Append(",");
                    else
                        isFirstActivity = false;
                    vrActivity.Settings.ThrowIfNull("vrActivity.Settings");
                    string childActivityCode = vrActivity.Settings.GenerateWFActivityCode(context);
                    codeBuilder.Append(childActivityCode);
                    codeBuilder.AppendLine();
                }

                codeBuilder.AppendLine();
                codeBuilder.Append("}");
            }

            codeBuilder.AppendLine();
            codeBuilder.Append("}");
            return codeBuilder.ToString();
        }
    }
}
