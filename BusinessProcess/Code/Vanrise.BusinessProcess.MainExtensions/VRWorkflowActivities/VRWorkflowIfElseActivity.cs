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
        public override Guid ConfigId { get { return new Guid("40B7E3E9-F8E0-4C2C-9ED7-F79CC4A68473"); } }

        public override string Editor { get { return "vr-workflowactivity-ifelse"; } }

        public override string Title { get { return "IfElse"; } }

        public string Condition { get; set; }

        public VRWorkflowActivity TrueActivity { get; set; }

        public VRWorkflowActivity FalseActivity { get; set; }

        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
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
            nmSpaceCodeBuilder.Replace("#CONDITION#", this.Condition);

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
                codeBuilder.Replace("#FALSEACTIVITYCODE#", this.FalseActivity.Settings.GenerateWFActivityCode(context));
            }

            codeBuilder.Append(@"
            }");

            codeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            codeBuilder.Replace("#CLASSNAME#", className);
            codeBuilder.Replace("#TRUEACTIVITYCODE#", this.TrueActivity.Settings.GenerateWFActivityCode(context));
            return codeBuilder.ToString();
        }
    }
}