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

        public override string Title { get { return "Subprocess"; } }

        public Guid VRWorkflowId { get; set; }

        public Dictionary<string, string> InArguments { get; set; }

        public Dictionary<string, string> OutArguments { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
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
                foreach (var argument in vrWorkflow.Settings.Arguments)
                {
                    String argumentRuntimeType = CSharpCompiler.TypeToString(argument.Type.GetRuntimeType(null));
                    string inArgumentValue = this.InArguments != null ? this.InArguments.GetRecord(argument.Name) : null;
                    string outArgumentValue = this.OutArguments != null ? this.OutArguments.GetRecord(argument.Name) : null;

                    switch (argument.Direction)
                    {
                        case VRWorkflowArgumentDirection.In:
                            if (!string.IsNullOrEmpty(inArgumentValue))
                            {
                                propertiesBuilder.AppendLine(string.Concat("public ", argumentRuntimeType, " In", argument.Name, "Prop { get { return ", inArgumentValue, "; } set { } }"));
                                argumentsList.Add(string.Format("{0} = new System.Activities.InArgument<{1}>((activityContext) => new {2}.{3}(activityContext).In{4}Prop)", argument.Name, argumentRuntimeType, nmSpaceName, className, argument.Name));
                            }
                            break;

                        case VRWorkflowArgumentDirection.InOut:
                            if (string.IsNullOrEmpty(inArgumentValue) && string.IsNullOrEmpty(outArgumentValue))
                                break;

                            if (!string.IsNullOrEmpty(inArgumentValue) && !string.IsNullOrEmpty(outArgumentValue))
                            {
                                propertiesBuilder.AppendLine(string.Concat("public ", argumentRuntimeType, " In", argument.Name, "Prop { get { return ", inArgumentValue, "; } set { ", outArgumentValue, " = value; } }"));
                                argumentsList.Add(string.Format("{0} = new System.Activities.InOutArgument<{1}>((activityContext) => new {2}.{3}(activityContext).In{4}Prop)", argument.Name, argumentRuntimeType, nmSpaceName, className, argument.Name));
                                break;
                            }

                            if (!string.IsNullOrEmpty(inArgumentValue))
                            {
                                propertiesBuilder.AppendLine(string.Concat("public ", argumentRuntimeType, " In", argument.Name, "Prop { get { return ", inArgumentValue, "; } set { } }"));
                                argumentsList.Add(string.Format("{0} = new System.Activities.InOutArgument<{1}>((activityContext) => new {2}.{3}(activityContext).In{4}Prop)", argument.Name, argumentRuntimeType, nmSpaceName, className, argument.Name));
                                break;
                            }

                            //outArgumnetValue not null
                            propertiesBuilder.AppendLine(string.Concat("public ", argumentRuntimeType, " Out", argument.Name, "Prop { get { return default(",argumentRuntimeType,");} set { ", outArgumentValue, " = value; } }"));
                            argumentsList.Add(string.Format("{0} = new System.Activities.InOutArgument<{1}>((activityContext) =>new {2}.{3}(activityContext).Out{4}Prop)", argument.Name, argumentRuntimeType, nmSpaceName, className, argument.Name));
                            break;

                        case VRWorkflowArgumentDirection.Out:
                            if (!string.IsNullOrEmpty(outArgumentValue))
                            {
                                propertiesBuilder.AppendLine(string.Concat("public ", argumentRuntimeType, " Out", argument.Name, "Prop { get { return default(", argumentRuntimeType, ");} set { ", outArgumentValue, " = value; } }"));
                                argumentsList.Add(string.Format("{0} = new System.Activities.OutArgument<{1}>((activityContext) =>new {2}.{3}(activityContext).Out{4}Prop)", argument.Name, argumentRuntimeType, nmSpaceName, className, argument.Name));
                            }
                            break;
                    }
                }
            }

            nmSpaceCodeBuilder.Replace("#PROPERTIES#", propertiesBuilder.ToString());
            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            System.Activities.Activity activity = new Vanrise.BusinessProcess.Business.VRWorkflowManager().GetVRWorkflowActivity(VRWorkflowId);
            string activityType = CSharpCompiler.TypeToString(activity.GetType());

            return string.Format("new {0}(){1}", activityType, argumentsList.Count > 0 ? string.Format("{0}{1}{2}", "{", string.Join<string>(",", argumentsList), "}") : string.Empty);
        }
    }
}
