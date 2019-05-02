using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowInsertVisualEventActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId { get { return new Guid("6651A08C-F337-49FE-A844-F30F052D4F1E"); } }

        public override string Editor { get { return "businessprocess-vr-workflowactivity-insertvisualevent"; } }

        public override string Title { get { return "Insert Visual Event"; } }

        public string DisplayName { get; set; }

        public VRWorkflowExpression EventTitle { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder nmSpaceCodeBuilder = new StringBuilder(@"                 

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : BaseCodeActivity
                    {
                        protected override void VRExecute(IBaseCodeActivityContext context)
                        {
                            var executionContext = new #CLASSNAME#ExecutionContext(context.ActivityContext);
                            executionContext.Execute();
                        }
                    }

                    #BASEEXECUTIONCLASSCODE#                  

                    public class #CLASSNAME#ExecutionContext : #BASEEXECUTIONCLASSNAME#
                    {
                        ActivityContext _activityContext;
                        public #CLASSNAME#ExecutionContext(ActivityContext activityContext) 
                            : base (activityContext)
                        {
                            _activityContext = activityContext;
                        }

                        public void Execute()
                        {
                            #INSERTVISUALEVENTCODE#
                        }
                    }
                }");

            var insertVisualEventInput = new GenerateInsertVisualEventCodeInput
            {
                ActivityContextVariableName = "_activityContext",
                ActivityId = context.VRWorkflowActivityId,
                EventTitle = this.EventTitle.GetCode(null),
                EventTypeId = CodeGenerationHelper.VISUALEVENTTYPE_COMPLETED
            };
            nmSpaceCodeBuilder.Replace("#INSERTVISUALEVENTCODE#",
                context.GenerateInsertVisualEventCode(insertVisualEventInput));
            
            string baseExecutionContextClassName = "InsertVisualEventActivity_BaseExecutionContext";
            string baseExecutionContextClassCode = CodeGenerationHelper.GenerateBaseExecutionClass(context, baseExecutionContextClassName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSCODE#", baseExecutionContextClassCode);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSNAME#", baseExecutionContextClassName);
            string codeNamespace = context.GenerateUniqueNamespace("CustomLogicActivity");
            string className = "CustomLogicWFActivity";
            string fullTypeName = string.Concat(codeNamespace, ".", className);

            nmSpaceCodeBuilder.Replace("#NAMESPACE#", codeNamespace);
            nmSpaceCodeBuilder.Replace("#CLASSNAME#", className);
            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            return string.Format("new {0}()", fullTypeName);
        }

        public override BPVisualItemDefinition GetVisualItemDefinition(IVRWorkflowActivityGetVisualItemDefinitionContext context)
        {
            return new BPVisualItemDefinition
            {
                Settings = new BPInsertVisualItemVisualItemDefinitionSettings
                {
                    DisplayName = this.DisplayName
                }
            };
        }
    }
    
    public class BPInsertVisualItemVisualItemDefinitionSettings : BPVisualItemDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("269E1AB2-3236-4EA4-90EA-FE9B1E27BF78"); } }

        public override string Editor { get { return "bp-workflow-activitysettings-visualitemdefiniton-insertvisualitem"; } }

        public string DisplayName { get; set; }
    }
}
