using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowWhileActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId => new Guid("0DEFE6D7-D4C3-4E73-804A-EE6764CCE57D");

        public override string Editor => throw new NotImplementedException();

        public override string Title => "While";

        public VRWorkflowExpression Condition { get; set; }

        public VRWorkflowActivity Activity { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            this.Condition.ThrowIfNull("this.Condition");
            this.Activity.ThrowIfNull("this.Activity");
            this.Activity.Settings.ThrowIfNull("this.Activity.Settings");

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

            string nmSpaceName = context.GenerateUniqueNamespace("whileActivity");
            string baseExecutionClassName = "whileActivityBaseExecutionContext";
            string baseExecutionClassCode = CodeGenerationHelper.GenerateBaseExecutionClass(context, baseExecutionClassName);
            string className = "whileActivityConditionExecutionContext";
            nmSpaceCodeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSNAME#", baseExecutionClassName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSCODE#", baseExecutionClassCode);
            nmSpaceCodeBuilder.Replace("#CLASSNAME#", className);
            nmSpaceCodeBuilder.Replace("#CONDITION#", this.Condition.GetCode(null));

            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.Append(@"
            new System.Activities.Statements.While((activityContext) => new #NAMESPACE#.#CLASSNAME#(activityContext).Evaluate())
            {
                Body = #ACTIVITYCODE#
            }");

            codeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            codeBuilder.Replace("#CLASSNAME#", className);
            var activityContext = context.CreateChildContext(null);
            activityContext.VRWorkflowActivityId = this.Activity.VRWorkflowActivityId;
            codeBuilder.Replace("#ACTIVITYCODE#", this.Activity.Settings.GenerateWFActivityCode(activityContext));
            return codeBuilder.ToString();
        }
    }
}
