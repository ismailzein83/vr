using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowCustomLogicActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId
		{
            get { return new Guid("F8D16729-620A-4780-80DD-2C37F1ED6A3C"); }
		}

		public override string Editor
		{
			get { return "businessprocess-vr-workflow-customcode"; }
		}

		public override string Title
		{
			get { return "Custom Code"; }
		}

		public string Code { get; set; }

        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
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
                        public #CLASSNAME#ExecutionContext(ActivityContext activityContext) 
                            : base (activityContext)
                        {
                        }

                        public void Execute()
                        {
                            #CUSTOMCODE#
                        }
                    }
                }");

            nmSpaceCodeBuilder.Replace("#CUSTOMCODE#", this.Code);
            string baseExecutionContextClassName = "CustomLogicWFActivity_BaseExecutionContext";
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
    }
}
