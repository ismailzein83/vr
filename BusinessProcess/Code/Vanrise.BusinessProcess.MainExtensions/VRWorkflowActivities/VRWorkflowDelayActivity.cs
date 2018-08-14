using System;
using System.Text;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowDelayActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId { get { return new Guid("FEABAAFD-C3E9-4DBB-AABF-45DB33D33517"); } }

        public override string Editor { get { return "businessprocess-vr-workflowactivity-delay"; } }

        public override string Title { get { return "Delay"; } }

        public string Delay { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder nmSpaceCodeBuilder = new StringBuilder(@"                 
                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : NativeActivity
                    {
                        protected override void Execute(NativeActivityContext context)
                        {
                            var executionContext = new #CLASSNAME#ExecutionContext(context);
                            executionContext.Execute();
                        }

                        protected override bool CanInduceIdle
                        {
                            get
                            {
                                return true;
                            }
                        }
                    }

                    #BASEEXECUTIONCLASSCODE#                  

                    public class #CLASSNAME#ExecutionContext : #BASEEXECUTIONCLASSNAME#
                    {
                        NativeActivityContext _activityContext;
                        public #CLASSNAME#ExecutionContext(NativeActivityContext activityContext) 
                            : base (activityContext)
                        {
                            _activityContext = activityContext;
                        }

                        public void Execute()
                        {

                            long processInstanceId = _activityContext.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
                            string bookmarkName = GetWFBookmark(processInstanceId);
                            new Vanrise.BusinessProcess.Business.BPTimeSubscriptionManager().InsertBPTimeSubscription(processInstanceId, bookmarkName, #DELAY#);
                            _activityContext.CreateBookmark(bookmarkName);
                        }

                        string GetWFBookmark(long processInstanceId)
                        {
                            return string.Format(""Delay_{0}_{1}"", processInstanceId, Guid.NewGuid());
                        }
                    }
                }");

            string baseExecutionContextClassName = "DelayActivity_BaseExecutionContext";
            string baseExecutionContextClassCode = CodeGenerationHelper.GenerateBaseExecutionClass(context, baseExecutionContextClassName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSCODE#", baseExecutionContextClassCode);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSNAME#", baseExecutionContextClassName);
            
            nmSpaceCodeBuilder.Replace("#DELAY#", this.Delay);

            string codeNamespace = context.GenerateUniqueNamespace("DelayActivity");
            string className = "DelayActivity";
            string fullTypeName = string.Concat(codeNamespace, ".", className);

            nmSpaceCodeBuilder.Replace("#NAMESPACE#", codeNamespace);
            nmSpaceCodeBuilder.Replace("#CLASSNAME#", className);
            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            return string.Format("new {0}()", fullTypeName);
        }
    }
}