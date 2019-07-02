using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowForEachActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId { get { return new Guid("BA3A107D-20DA-4456-AC70-A2948DFE3725"); } }

        public override string Editor { get { return "businessprocess-vr-workflowactivity-foreach"; } }

        public override string Title { get { return "ForEach"; } }

        [Newtonsoft.Json.JsonConverter(typeof(VRWorkflowExpressionJsonConverter))]
        public VRWorkflowExpression List { get; set; }

        public string IterationVariableName { get; set; }

        public VRWorkflowVariableType IterationVariableType { get; set; }

        public VRWorkflowActivity Activity { get; set; }

        public string Description { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            //System.Activities.Statements.ForEach<string> foreachActivity = new System.Activities.Statements.ForEach<string>
            //{
            //    Values = new System.Activities.InArgument<IEnumerable<string>>((activityContext) => new List<string> { "item 1", "item 2"}),
            //    Body = new System.Activities.ActivityAction<string>
            //    {
            //         Argument = new System.Activities.DelegateInArgument<string>("item")

            //    }
            //};

            this.List.ThrowIfNull("List");
            this.IterationVariableName.ThrowIfNull("IterationVariableName");
            this.IterationVariableType.ThrowIfNull("IterationVariableType");
            this.Activity.ThrowIfNull("Activity");
            this.Activity.Settings.ThrowIfNull("Activity.Settings");

            var childContext = context.CreateChildContext(null);
            childContext.AddVariable(new VRWorkflowVariable { VRWorkflowVariableId = Guid.NewGuid(), Name = this.IterationVariableName, Type = this.IterationVariableType });

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

                        public IEnumerable<#ITERATIONVARIABLERUNTIMETYPE#> List
                        {
                            get
                            {
                                return #LISTNAME#;
                            }
                        }
                    }

                    public class #CLASSNAME#_InsertNewIterationVisualEventActivity : BaseCodeActivity
                    {
                        protected override void VRExecute(IBaseCodeActivityContext context)
                        {
                            var executionContext = new #CLASSNAME#_InsertNewIterationVisualEventActivity_ExecutionContext(context.ActivityContext);
                            executionContext.Execute();
                        }
                    }

                    public class #CLASSNAME#_InsertNewIterationVisualEventActivity_ExecutionContext : #BASEEXECUTIONCLASSNAME#
                    {
                        ActivityContext _activityContext;
                        public #CLASSNAME#_InsertNewIterationVisualEventActivity_ExecutionContext(ActivityContext activityContext) 
                            : base (activityContext)
                        {
                            _activityContext = activityContext;
                        }

                        public void Execute()
                        {
                            #INSERTNEWITERATIONVISUALEVENTCODE#
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

            String iterationVariableRuntimeType = CSharpCompiler.TypeToString(this.IterationVariableType.GetRuntimeType(null));

            string nmSpaceName = context.GenerateUniqueNamespace("ForEach");
            string baseExecutionClassName = "ForEachBaseExecutionContext";
            string baseExecutionClassCode = CodeGenerationHelper.GenerateBaseExecutionClass(context, baseExecutionClassName);
            string className = "ForEachExecutionContext";
            nmSpaceCodeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSNAME#", baseExecutionClassName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSCODE#", baseExecutionClassCode);
            nmSpaceCodeBuilder.Replace("#CLASSNAME#", className);
            nmSpaceCodeBuilder.Replace("#LISTNAME#", this.List.GetCode(null));
            nmSpaceCodeBuilder.Replace("#ITERATIONVARIABLERUNTIMETYPE#", iterationVariableRuntimeType);

            var insertNewIterationVisualEventInput = new GenerateInsertVisualEventCodeInput
            {
                ActivityContextVariableName = "_activityContext",
                ActivityId = context.VRWorkflowActivityId,
                EventTitle = @"""ForEach Activity New Iteration""",
                EventTypeId = CodeGenerationHelper.VISUALEVENTTYPE_NEWITERATION
            };
            nmSpaceCodeBuilder.Replace("#INSERTNEWITERATIONVISUALEVENTCODE#",
                context.GenerateInsertVisualEventCode(insertNewIterationVisualEventInput));

            var insertCompletedVisualEventInput = new GenerateInsertVisualEventCodeInput
            {
                ActivityContextVariableName = "_activityContext",
                ActivityId = context.VRWorkflowActivityId,
                EventTitle = $@"""ForEach Activity completed""",
                EventTypeId = CodeGenerationHelper.VISUALEVENTTYPE_COMPLETED
            };
            nmSpaceCodeBuilder.Replace("#INSERTCOMPLETEDVISUALEVENTCODE#",
                context.GenerateInsertVisualEventCode(insertCompletedVisualEventInput));


            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.Append(@"
                    new Sequence
                        {
                            Activities =
                            {
                                new System.Activities.Statements.ForEach<#ITERATIONVARIABLERUNTIMETYPE#>
                                {
                                    Values =  new System.Activities.InArgument<IEnumerable<#ITERATIONVARIABLERUNTIMETYPE#>>((activityContext) => new #NAMESPACE#.#CLASSNAME#(activityContext).List),
                                    Body = new System.Activities.ActivityAction<#ITERATIONVARIABLERUNTIMETYPE#>
                                                    {
                                                         Argument = new System.Activities.DelegateInArgument<#ITERATIONVARIABLERUNTIMETYPE#>(""#ITERATIONVARIABLENAME#""),
                                                         Handler = new Sequence
                                                                {
                                                                    Activities =
                                                                    {
                                                                        new #NAMESPACE#.#CLASSNAME#_InsertNewIterationVisualEventActivity(),
                                                                        #ACTIVITYCODE#
                                                                    }
                                                                }                    
                                                    }
                                },
                                new #NAMESPACE#.#CLASSNAME#_InsertCompletedVisualEventActivity()
                            }
                        }

            ");

            codeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            codeBuilder.Replace("#CLASSNAME#", className);
            codeBuilder.Replace("#ITERATIONVARIABLERUNTIMETYPE#", iterationVariableRuntimeType);
            codeBuilder.Replace("#ITERATIONVARIABLENAME#", this.IterationVariableName);

            var newContext = childContext.CreateChildContext(context.GenerateInsertVisualEventCode);
            newContext.VRWorkflowActivityId = this.Activity.VRWorkflowActivityId;

            codeBuilder.Replace("#ACTIVITYCODE#", this.Activity.Settings.GenerateWFActivityCode(newContext));
            return codeBuilder.ToString();
        }

        public override BPVisualItemDefinition GetVisualItemDefinition(IVRWorkflowActivityGetVisualItemDefinitionContext context)
        {
            if (this.Activity != null && this.Activity.Settings != null)
            {
                var childActivityVisualItem = this.Activity.Settings.GetVisualItemDefinition(context);
                if (childActivityVisualItem != null)
                {
                    return new BPVisualItemDefinition
                    {
                        Settings = new BPForEachVisualItemDefinition
                        {
                            Description = this.Description,
                            ChildActivityId = this.Activity.VRWorkflowActivityId,
                            ChildActivityVisualItemDefinition = childActivityVisualItem
                        }
                    };
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }


    public class BPForEachVisualItemDefinition : BPVisualItemDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("8D691741-D0B6-4CF9-8A33-998E0DCC0B0D"); } }

        public override string Editor { get { return "bp-workflow-activitysettings-visualitemdefiniton-foreach"; } }

        public string Description { get; set; }

        public Guid ChildActivityId { get; set; }

        public BPVisualItemDefinition ChildActivityVisualItemDefinition { get; set; }
    }
}
