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

        public Dictionary<string, VRWorkflowExpression> InArguments { get; set; }

        public Dictionary<string, VRWorkflowExpression> OutArguments { get; set; }

        public string DisplayName { get; set; }

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

                    public class #CLASSNAME#_InsertStartedVisualEventActivity : BaseCodeActivity
                    {
                        protected override void VRExecute(IBaseCodeActivityContext context)
                        {
                            var executionContext = new #CLASSNAME#_InsertStartedVisualEventActivity_ExecutionContext(context.ActivityContext);
                            executionContext.Execute();
                        }
                    }

                    public class #CLASSNAME#_InsertStartedVisualEventActivity_ExecutionContext : #BASEEXECUTIONCLASSNAME#
                    {
                        ActivityContext _activityContext;
                        public #CLASSNAME#_InsertStartedVisualEventActivity_ExecutionContext(ActivityContext activityContext) 
                            : base (activityContext)
                        {
                            _activityContext = activityContext;
                        }

                        public void Execute()
                        {
                            #INSERTSTARTEDVISUALEVENTCODE#
                        }
                    }

                    public class #CLASSNAME#_InsertCompletedVisualEventActivity : BaseCodeActivity
                    {
                        protected override void VRExecute(IBaseCodeActivityContext context)
                        {
                            var executionContext = new #CLASSNAME#_InsertCompletedVisualEventActivity_ExecutionContext(context.ActivityContext);
                            executionContext.Execute();
                        }
                    }

                    public class #CLASSNAME#_InsertCompletedVisualEventActivity_ExecutionContext : #BASEEXECUTIONCLASSNAME#
                    {
                        ActivityContext _activityContext;
                        public #CLASSNAME#_InsertCompletedVisualEventActivity_ExecutionContext(ActivityContext activityContext) 
                            : base (activityContext)
                        {
                            _activityContext = activityContext;
                        }

                        public void Execute()
                        {
                            #INSERTCOMPLETEDVISUALEVENTCODE#
                        }
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
            var vrWorkflow = new VRWorkflowManager().GetVRWorkflow(this.VRWorkflowId);
            string displayName = string.IsNullOrEmpty(this.DisplayName) ? vrWorkflow.Name : this.DisplayName;

            var insertStartedVisualEventInput = new GenerateInsertVisualEventCodeInput
            {
                ActivityContextVariableName = "_activityContext",
                ActivityId = context.VRWorkflowActivityId,
                EventTitle = $@"""Sub Process '{displayName}' started""",
                EventTypeId = CodeGenerationHelper.VISUALEVENTTYPE_STARTED
            };
            nmSpaceCodeBuilder.Replace("#INSERTSTARTEDVISUALEVENTCODE#",
                context.GenerateInsertVisualEventCode(insertStartedVisualEventInput));

            var insertCompletedVisualEventInput = new GenerateInsertVisualEventCodeInput
            {
                ActivityContextVariableName = "_activityContext",
                ActivityId = context.VRWorkflowActivityId,
                EventTitle = $@"""Sub Process '{displayName}' completed""",
                EventTypeId = CodeGenerationHelper.VISUALEVENTTYPE_COMPLETED
            };
            nmSpaceCodeBuilder.Replace("#INSERTCOMPLETEDVISUALEVENTCODE#",
                context.GenerateInsertVisualEventCode(insertCompletedVisualEventInput));

            StringBuilder propertiesBuilder = new StringBuilder();
            List<string> argumentsList = new List<string>();


            if (vrWorkflow.Settings.Arguments != null && vrWorkflow.Settings.Arguments.Any())
            {
                foreach (var argument in vrWorkflow.Settings.Arguments)
                {
                    String argumentRuntimeType = argument.Type.GetRuntimeTypeAsString(null);
                    VRWorkflowExpression inArgumentValue = this.InArguments != null ? this.InArguments.GetRecord(argument.Name) : null;
                    VRWorkflowExpression outArgumentValue = this.OutArguments != null ? this.OutArguments.GetRecord(argument.Name) : null;

                    switch (argument.Direction)
                    {
                        case VRWorkflowArgumentDirection.In:
                            if (inArgumentValue != null)
                            {
                                propertiesBuilder.AppendLine(string.Concat("public ", argumentRuntimeType, " In", argument.Name, "Prop { get { return ", inArgumentValue.GetCode(null), "; } set { } }"));
                                argumentsList.Add(string.Format("{0} = new System.Activities.InArgument<{1}>((activityContext) => new {2}.{3}(activityContext).In{4}Prop)", argument.Name, argumentRuntimeType, nmSpaceName, className, argument.Name));
                            }
                            break;

                        case VRWorkflowArgumentDirection.InOut:
                            if (inArgumentValue == null && outArgumentValue == null)
                                break;

                            if (inArgumentValue != null && outArgumentValue != null)
                            {
                                propertiesBuilder.AppendLine(string.Concat("public ", argumentRuntimeType, " In", argument.Name, "Prop { get { return ", inArgumentValue.GetCode(null), "; } set { ", outArgumentValue.GetCode(null), " = value; } }"));
                                argumentsList.Add(string.Format("{0} = new System.Activities.InOutArgument<{1}>((activityContext) => new {2}.{3}(activityContext).In{4}Prop)", argument.Name, argumentRuntimeType, nmSpaceName, className, argument.Name));
                                break;
                            }

                            if (inArgumentValue != null)
                            {
                                propertiesBuilder.AppendLine(string.Concat("public ", argumentRuntimeType, " In", argument.Name, "Prop { get { return ", inArgumentValue.GetCode(null), "; } set { } }"));
                                argumentsList.Add(string.Format("{0} = new System.Activities.InOutArgument<{1}>((activityContext) => new {2}.{3}(activityContext).In{4}Prop)", argument.Name, argumentRuntimeType, nmSpaceName, className, argument.Name));
                                break;
                            }

                            //outArgumnetValue not null
                            propertiesBuilder.AppendLine(string.Concat("public ", argumentRuntimeType, " Out", argument.Name, "Prop { get { return default(",argumentRuntimeType,");} set { ", outArgumentValue.GetCode(null), " = value; } }"));
                            argumentsList.Add(string.Format("{0} = new System.Activities.InOutArgument<{1}>((activityContext) =>new {2}.{3}(activityContext).Out{4}Prop)", argument.Name, argumentRuntimeType, nmSpaceName, className, argument.Name));
                            break;

                        case VRWorkflowArgumentDirection.Out:
                            if (outArgumentValue != null)
                            {
                                propertiesBuilder.AppendLine(string.Concat("public ", argumentRuntimeType, " Out", argument.Name, "Prop { get { return default(", argumentRuntimeType, ");} set { ", outArgumentValue.GetCode(null), " = value; } }"));
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

            //var subProcessVRWorkflow = new Vanrise.BusinessProcess.Business.VRWorkflowManager().GetVRWorkflow(VRWorkflowId);
            //subProcessVRWorkflow.ThrowIfNull("subProcessVRWorkflow", VRWorkflowId);

            //string subProcessVRWorkflowClassName = subProcessVRWorkflow.Name.Replace(" ", "_").Replace("-", "_").Replace("(", "_").Replace(")", "_");
            //string activityType = $"Vanrise.DynWorkflows.{subProcessVRWorkflowClassName}";

            StringBuilder codeBuilder = new StringBuilder(@"new Sequence
                                    {
                                        Activities =
                                        {
                                            new #NAMESPACE#.#CLASSNAME#_InsertStartedVisualEventActivity(),
                                            new #ACTIVITYTYPE#()#ARGUMENTS#,
                                            new #NAMESPACE#.#CLASSNAME#_InsertCompletedVisualEventActivity()
                                        }
                                    }
                            ");

            codeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            codeBuilder.Replace("#CLASSNAME#", className);
            codeBuilder.Replace("#ACTIVITYTYPE#", activityType);
            codeBuilder.Replace("#ARGUMENTS#", argumentsList.Count > 0 ? string.Format("{0}{1}{2}", "{", string.Join<string>(",", argumentsList), "}") : string.Empty);

            return codeBuilder.ToString();
        }

        public override BPVisualItemDefinition GetVisualItemDefinition(IVRWorkflowActivityGetVisualItemDefinitionContext context)
        {
            //VRWorkflow vrWorkflow = new VRWorkflowManager().GetVRWorkflow(this.VRWorkflowId);
            //string displayName = this.DisplayName;
            //if (context.SubProcessActivityName != null)
            //    displayName = displayName.Replace("[SubProcessActivityName]", context.SubProcessActivityName);
            //return vrWorkflow?.Settings?.RootActivity?.Settings?.GetVisualItemDefinition(new VRWorkflowActivityGetVisualItemDefinitionContext(context, displayName));

            VRWorkflow vrWorkflow = new VRWorkflowManager().GetVRWorkflow(this.VRWorkflowId);
            string displayName = string.IsNullOrEmpty(this.DisplayName) ? vrWorkflow.Name : this.DisplayName;
            if (context.SubProcessActivityName != null)
                displayName = displayName.Replace("[SubProcessActivityName]", context.SubProcessActivityName);
            var childVisualItemDefinition = vrWorkflow?.Settings?.RootActivity?.Settings?.GetVisualItemDefinition(new VRWorkflowActivityGetVisualItemDefinitionContext(context, displayName));
            if (childVisualItemDefinition != null)
            {
                return new BPVisualItemDefinition
                {
                    Settings = new BPSubProcessVisualItemDefinition
                    {
                        ChildActivityId = vrWorkflow.Settings.RootActivity.VRWorkflowActivityId,
                        ChildActivityVisualItemDefinition = childVisualItemDefinition
                    }
                };
            }
            else
            {
                return null;
            }
        }

        private class VRWorkflowActivityGetVisualItemDefinitionContext : IVRWorkflowActivityGetVisualItemDefinitionContext
        {
            IVRWorkflowActivityGetVisualItemDefinitionContext _parentContext;

            string _displayName;
            public VRWorkflowActivityGetVisualItemDefinitionContext(IVRWorkflowActivityGetVisualItemDefinitionContext parentContext, string displayName)
            {
                _parentContext = parentContext;
                _displayName = displayName;
            }

            public string SubProcessActivityName => this._displayName;
        }


    }

    public class BPSubProcessVisualItemDefinition : BPVisualItemDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("ECCDB81D-057C-493F-820B-80DCFECBBD93"); } }

        public override string Editor { get { return "bp-workflow-activitysettings-visualitemdefiniton-subprocess"; } }
        
        public Guid ChildActivityId { get; set; }

        public BPVisualItemDefinition ChildActivityVisualItemDefinition { get; set; }
    }
}
