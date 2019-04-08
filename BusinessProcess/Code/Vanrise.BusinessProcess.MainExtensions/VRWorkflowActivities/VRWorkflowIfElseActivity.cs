﻿using System;
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
        public override Guid ConfigId { get { return new Guid("40B7E3E9-F8E0-4C2C-9ED7-F79CC4A68473"); } }

        public override string Editor { get { return "businessprocess-vr-workflowactivity-ifelse"; } }

        public override string Title { get { return "If Else"; } }

        [Newtonsoft.Json.JsonConverter(typeof(VRWorkflowExpressionJsonConverter))]
        public VRWorkflowExpression Condition { get; set; }

        public string ConditionDescription { get; set; }

        public VRWorkflowActivity TrueActivity { get; set; }

        public VRWorkflowActivity FalseActivity { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            //System.Activities.Statements.If wfIfActivity = new System.Activities.Statements.If((activityContext) => false)
            //{ 
            //    Condition = new System.Activities.InArgument<bool>((activityContext) => false),
            //    Then = new System.Activities.Statements.If(),
            //};

            //System.Activities.Statements.ForEach<int> foreachActivity = new System.Activities.Statements.ForEach<int>
            //{
            //    Body = new System.Activities.ActivityAction<int> {  }
            //};

            this.Condition.ThrowIfNull("this.Condition");
            this.TrueActivity.ThrowIfNull("this.TrueActivity");
            this.TrueActivity.Settings.ThrowIfNull("this.TrueActivity.Settings");

            StringBuilder nmSpaceCodeBuilder = new StringBuilder(@"                 

                namespace #NAMESPACE#
                {
                    #BASEEXECUTIONCLASSCODE#                  

                    public class #CLASSNAME# : #BASEEXECUTIONCLASSNAME#
                    {
                        public #CLASSNAME#(ActivityContext activityContext) 
                            : base (activityContext)
                        {
                        }

                        public bool Evaluate()
                        {
                            return #CONDITION#;
                        }
                    }
                }");

            string nmSpaceName = context.GenerateUniqueNamespace("IfElse");
            string baseExecutionClassName = "IfElseBaseExecutionContext";
            string baseExecutionClassCode = CodeGenerationHelper.GenerateBaseExecutionClass(context, baseExecutionClassName);
            string className = "IfConditionExecutionContext";
            nmSpaceCodeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSNAME#", baseExecutionClassName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSCODE#", baseExecutionClassCode);
            nmSpaceCodeBuilder.Replace("#CLASSNAME#", className);
            nmSpaceCodeBuilder.Replace("#CONDITION#", this.Condition.GetCode(null));

            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.Append(@"
            new System.Activities.Statements.If((activityContext) => new #NAMESPACE#.#CLASSNAME#(activityContext).Evaluate())
            {
                Then = #TRUEACTIVITYCODE#");

            if (this.FalseActivity != null)
            {
                this.FalseActivity.Settings.ThrowIfNull("this.FalseActivity.Settings");
                codeBuilder.Append(@",
            Else = #FALSEACTIVITYCODE#");
                var falseContext = context.CreateChildContext(context.GenerateInsertVisualEventCode);
                falseContext.VRWorkflowActivityId = this.FalseActivity.VRWorkflowActivityId;
                codeBuilder.Replace("#FALSEACTIVITYCODE#", this.FalseActivity.Settings.GenerateWFActivityCode(falseContext));
            }

            codeBuilder.Append(@"
            }");

            codeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            codeBuilder.Replace("#CLASSNAME#", className);
            var trueContext = context.CreateChildContext(context.GenerateInsertVisualEventCode);
            trueContext.VRWorkflowActivityId = this.TrueActivity.VRWorkflowActivityId;
            codeBuilder.Replace("#TRUEACTIVITYCODE#", this.TrueActivity.Settings.GenerateWFActivityCode(trueContext));
            return codeBuilder.ToString();
        }

        public override BPVisualItemDefinition GetVisualItemDefinition(IVRWorkflowActivityGetVisualItemDefinitionContext context)
        {
            this.Condition.ThrowIfNull("this.Condition");
            this.TrueActivity.ThrowIfNull("this.TrueActivity");
            this.TrueActivity.Settings.ThrowIfNull("this.TrueActivity.Settings");
            var trueBranchVisualItemDef = this.TrueActivity.Settings.GetVisualItemDefinition(context);
            BPVisualItemDefinition falseBranchVisualItemDef = null;
            if (this.FalseActivity != null)
            {
                this.FalseActivity.Settings.ThrowIfNull("this.FalseActivity.Settings");
                falseBranchVisualItemDef = this.FalseActivity.Settings.GetVisualItemDefinition(context);
            }
            if (trueBranchVisualItemDef != null || falseBranchVisualItemDef != null)
            {
                return new BPVisualItemDefinition
                {
                    Settings = new BPIfElseVisualItemDefinition
                    {
                        ConditionDescription = !String.IsNullOrEmpty(this.ConditionDescription) ? this.ConditionDescription : this.Condition.GetCode(null),
                        TrueBranchActivityId = trueBranchVisualItemDef != null ? this.TrueActivity.VRWorkflowActivityId : default(Guid?),
                        TrueBranchVisualItemDefinition = trueBranchVisualItemDef,
                        FalseBranchActivityId= falseBranchVisualItemDef != null ? this.FalseActivity.VRWorkflowActivityId : default(Guid?),
                        FalseBranchVisualItemDefinition = falseBranchVisualItemDef
                    }
                };
            }
            else
            {
                return null;
            }
        }
    }

    public class BPIfElseVisualItemDefinition : BPVisualItemDefinitionSettings
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public override string Editor => throw new NotImplementedException();

        public string ConditionDescription { get; set; }

        public Guid? TrueBranchActivityId { get; set; }

        public BPVisualItemDefinition TrueBranchVisualItemDefinition { get; set; }

        public Guid? FalseBranchActivityId { get; set; }

        public BPVisualItemDefinition FalseBranchVisualItemDefinition { get; set; }
    }
}