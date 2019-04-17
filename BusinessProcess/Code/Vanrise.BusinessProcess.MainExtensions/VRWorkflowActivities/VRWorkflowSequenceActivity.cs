using System;
using System.Text;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowSequenceActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId { get { return new Guid("9292B3BE-256F-400F-9BC6-A0423FA0B30F"); } }

        public override string Editor { get { return "businessprocess-vr-workflowactivity-sequence"; } }

        public override string Title { get { return "Sequence"; } }

        public VRWorkflowActivityCollection Activities { get; set; }

        public VRWorkflowVariableCollection Variables { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();

            //WF Sequence activity is needed only when more than 1 activity exist or Variables exists
            if ((this.Activities != null && this.Activities.Count > 1) || (this.Variables != null && this.Variables.Count > 0))
            {
                var childContext = context.CreateChildContext(null);

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
                        var newContext = childContext.CreateChildContext(context.GenerateInsertVisualEventCode);
                        newContext.VRWorkflowActivityId = vrActivity.VRWorkflowActivityId;

                        vrActivity.Settings.ThrowIfNull("vrActivity.Settings", vrActivity.VRWorkflowActivityId);
                        if (vrActivity.Settings.IsDisabled)
                            continue;

                        if (!isFirstActivity)
                            codeBuilder.Append(",");
                        else
                            isFirstActivity = false;

                        string childActivityCode = vrActivity.Settings.GenerateWFActivityCode(newContext);
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
                if (this.Activities.Count == 1)
                {
                    var firstActivity = this.Activities[0];
                    firstActivity.Settings.ThrowIfNull("firstActivity.Settings");
                    var newContext = context.CreateChildContext(context.GenerateInsertVisualEventCode);
                    newContext.VRWorkflowActivityId = firstActivity.VRWorkflowActivityId;

                    codeBuilder.Append(firstActivity.Settings.GenerateWFActivityCode(newContext));
                }
            }
            return codeBuilder.ToString();
        }

        public override BPVisualItemDefinition GetVisualItemDefinition(IVRWorkflowActivityGetVisualItemDefinitionContext context)
        {
            List<VRWorkflowSequenceChildVisualItemDefinition> childVisualItemDefinitions = null;
            if (this.Activities != null)
            {
                foreach(var vrActivity in this.Activities)
                {
                    vrActivity.Settings.ThrowIfNull("vrActivity.Settings", vrActivity.VRWorkflowActivityId);
                    if (vrActivity.Settings.IsDisabled)
                        continue;
                    var childVisualItemDef = vrActivity.Settings.GetVisualItemDefinition(context);
                    if(childVisualItemDef != null)
                    {
                        if(childVisualItemDefinitions == null)
                        {
                            childVisualItemDefinitions = new List<VRWorkflowSequenceChildVisualItemDefinition>();
                        }
                        childVisualItemDefinitions.Add(new VRWorkflowSequenceChildVisualItemDefinition
                        {
                            ChildActivityId = vrActivity.VRWorkflowActivityId,
                            ChildItemDefinition = childVisualItemDef
                        });
                    }
                }
            }
           
            if(childVisualItemDefinitions != null)
            {
                return new BPVisualItemDefinition
                {
                    Settings = new BPSequenceVisualItemDefinition
                    {
                        ChildVisualItems = childVisualItemDefinitions
                    }
                };
            }
            else
            {
                return null;
            }
        }
    }

    public class BPSequenceVisualItemDefinition : BPVisualItemDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("B128968B-FAF8-4077-B5EA-AAF085C3765F"); } }

        public override string Editor { get { return "bp-workflow-activitysettings-visualitemdefiniton-sequence"; } }

        public List<VRWorkflowSequenceChildVisualItemDefinition> ChildVisualItems { get; set; }
    }

    public class VRWorkflowSequenceChildVisualItemDefinition
    {
        public Guid ChildActivityId { get; set; }

        public BPVisualItemDefinition ChildItemDefinition { get; set; }
    }
}