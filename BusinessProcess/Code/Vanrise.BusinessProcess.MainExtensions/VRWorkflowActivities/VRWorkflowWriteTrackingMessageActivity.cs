using System;
using System.Text;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowWriteTrackingMessageActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId { get { return new Guid("A0DE8C69-7427-4F95-9A4D-9ECD8658D7B2"); } }

        public override string Editor { get { return "businessprocess-vr-workflowactivity-writetrackingmessage"; } }

        public override string Title { get { return "Write Tracking Message"; } }

        public string Message { get; set; }

        public VRWorkflowTrackingMessageSeverityEnum Severity { get; set; }

        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            this.Message.ThrowIfNull("this.Message");
            StringBuilder nmSpaceCodeBuilder = new StringBuilder(@"                 
                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : CodeActivity
                    {
                        protected override void Execute(CodeActivityContext context)
                        {
                            var executionContext = new #CLASSNAME#ExecutionContext(context);
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
                            _activityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.#SEVERITY#, #MESSAGE#, null);
                        }
                    }
                }");

            string baseExecutionContextClassName = "WriteTrackingMessageActivity_BaseExecutionContext";
            string baseExecutionContextClassCode = CodeGenerationHelper.GenerateBaseExecutionClass(context, baseExecutionContextClassName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSCODE#", baseExecutionContextClassCode);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSNAME#", baseExecutionContextClassName);
            nmSpaceCodeBuilder.Replace("#MESSAGE#", this.Message);
            nmSpaceCodeBuilder.Replace("#SEVERITY#", this.Severity.ToString());
            string codeNamespace = context.GenerateUniqueNamespace("WriteTrackingMessageActivity");
            string className = "WriteTrackingMessageActivity";
            string fullTypeName = string.Concat(codeNamespace, ".", className);

            nmSpaceCodeBuilder.Replace("#NAMESPACE#", codeNamespace);
            nmSpaceCodeBuilder.Replace("#CLASSNAME#", className);
            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            return string.Format("new {0}()", fullTypeName);
        }
    }

    public enum VRWorkflowTrackingMessageSeverityEnum
    {
        Warning = 0,
        Information = 1,
    }
}