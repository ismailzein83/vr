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

		public override string Editor
		{
			get { return "businessprocess-vr-workflow-sequence"; }
		}

		public override string Title
		{
			get { return "Sequence"; }
		}

		public VRWorkflowActivityCollection Activities { get; set; }

        public VRWorkflowVariableCollection Variables { get; set; }
        
        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();

            if ((this.Activities != null && this.Activities.Count > 1)
                ||
                (this.Variables != null && this.Variables.Count > 0)
                )//WF Sequence activity is needed only when more than 1 activity exist or Variables exists
            {
                var childContext = context.CreateChildContext();

                codeBuilder.Append(@"
            new Sequence
            {");
                codeBuilder.AppendLine();

                if (this.Variables != null)
                {
                    childContext.AddVariables(this.Variables);
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
                        vrActivity.Settings.ThrowIfNull("vrActivity.Settings", vrActivity.VRWorkflowActivityId);
                        string childActivityCode = vrActivity.Settings.GenerateWFActivityCode(childContext);
                        codeBuilder.Append(childActivityCode);
                        codeBuilder.AppendLine();
                    }

                    codeBuilder.AppendLine();
                    codeBuilder.Append("}");
                }

                codeBuilder.AppendLine();
                codeBuilder.Append("}");
            }
            else
            {
                if(this.Activities.Count == 1)
                {
                    var firstActivity = this.Activities[0];
                    firstActivity.Settings.ThrowIfNull("firstActivity.Settings");
                    codeBuilder.Append(firstActivity.Settings.GenerateWFActivityCode(context));
                }
            }
            return codeBuilder.ToString();
        }
    }
}
