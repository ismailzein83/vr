using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowSubProcessActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId { get { return new Guid("173258F8-2AC9-4214-BCE2-D3DB6D902423"); } }

        public override string Editor { get { return "businessprocess-vr-workflowactivity-subprocess"; } }

        public override string Title { get { return "Execute Worklfow"; } }

        public Guid VRWorkflowId { get; set; }

        public Dictionary<string, string> InArguments { get; set; }

        public Dictionary<string, string> OutArguments { get; set; }

        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder nmSpaceCodeBuilder = new StringBuilder(@"                 

                namespace #NAMESPACE#
                {
                    #BASEEXECUTIONCLASSCODE#                  

                    public class #CLASSNAME# : #BASEEXECUTIONCLASSNAME#
                    {
                        ActivityContext _activityContext;
                        public #CLASSNAME#(ActivityContext activityContext) 
                            : base (activityContext)
                        {
                            _activityContext = activityContext;
                        }

                        #PROPERTIES#
                    }
                }");

            string nmSpaceName = context.GenerateUniqueNamespace("SubProcess");
            string baseExecutionClassName = "SubProcessBaseExecutionContext";
            string baseExecutionClassCode = CodeGenerationHelper.GenerateBaseExecutionClass(context, baseExecutionClassName);
            string className = "SubProcessExecutionContext";
            nmSpaceCodeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSNAME#", baseExecutionClassName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSCODE#", baseExecutionClassCode);
            nmSpaceCodeBuilder.Replace("#CLASSNAME#", className);

            StringBuilder propertiesBuilder = new StringBuilder();
            List<string> argumentsList = new List<string>();

            VRWorkflow vrWorkflow = new VRWorkflowManager().GetVRWorkflow(this.VRWorkflowId);

            if (vrWorkflow.Settings.Arguments != null && vrWorkflow.Settings.Arguments.Any())
            {
                Dictionary<string, VRWorkflowArgument> workflowArguments = vrWorkflow.Settings.Arguments.ToDictionary(itm => itm.Name, itm => itm);

                if (this.InArguments != null && this.InArguments.Any())
                {
                    foreach (var inArgument in this.InArguments)
                    {
                        VRWorkflowArgument matchingArgument = workflowArguments.GetRecord(inArgument.Key);
                        String matchingArgumentRuntimeType = CSharpCompiler.TypeToString(matchingArgument.Type.GetRuntimeType(null));

                        propertiesBuilder.AppendLine(string.Format("public {0} In{1}Prop { get { return {1}; } set;}", matchingArgumentRuntimeType, inArgument.Value));
                        argumentsList.Add(string.Format("{0} new System.Activities.InArgument<{1}>((activityContext) => new #NAMESPACE#.#CLASSNAME#(activityContext).In{2}Prop)", inArgument.Key, matchingArgumentRuntimeType, inArgument.Value));
                    }

                    foreach (var outArgument in this.OutArguments)
                    {
                        VRWorkflowArgument matchingArgument = workflowArguments.GetRecord(outArgument.Key);
                        String matchingArgumentRuntimeType = CSharpCompiler.TypeToString(matchingArgument.Type.GetRuntimeType(null));

                        propertiesBuilder.AppendLine(string.Format("public {0} Out{1}Prop { get; set { {1} = value;}}", matchingArgumentRuntimeType, outArgument.Value));
                        argumentsList.Add(string.Format("{0} new System.Activities.OutArgument<{1}>((activityContext) => new #NAMESPACE#.#CLASSNAME#(activityContext).Out{2}Prop)", outArgument.Key, matchingArgumentRuntimeType, outArgument.Value));
                    }
                }
            }

            nmSpaceCodeBuilder.Replace("#PROPERTIES#", propertiesBuilder.ToString());
            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            //StringBuilder codeBuilder = new StringBuilder();

            System.Activities.Activity activity = new Vanrise.BusinessProcess.Business.VRWorkflowManager().GetVRWorkflowActivity(VRWorkflowId);
            string activityType = CSharpCompiler.TypeToString(activity.GetType());

            //codeBuilder.AppendLine(@"new Vanrise.BusinessProcess.Business.VRWorkflowManager().GetVRWorkflowActivity(new Guid(""#VRWORKFLOWID#""))");
            //if (argumentsList.Count > 0)
            //    codeBuilder.AppendFormat("{{0}}", string.Join<string>(",", argumentsList));

            //codeBuilder.Replace("#VRWORKFLOWID#", this.VRWorkflowId.ToString());

            //return codeBuilder.ToString();

            //if (argumentsList.Count > 0)
            //    codeBuilder.AppendFormat("{{0}}", string.Join<string>(",", argumentsList));

            return string.Format("new {0}(){1}", activityType, argumentsList.Count > 0 ? string.Format("{{0}}", string.Join<string>(",", argumentsList)) : string.Empty);
        }
    }
}
