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

        public override string Editor { get { return "businessprocess-vr-workflowactivity-while"; }}

        public override string Title => "While";

        public VRWorkflowExpression Condition { get; set; }

        public string ConditionDescription { get; set; }

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

            string nmSpaceName = context.GenerateUniqueNamespace("whileActivity");
            string baseExecutionClassName = "whileActivityBaseExecutionContext";
            string baseExecutionClassCode = CodeGenerationHelper.GenerateBaseExecutionClass(context, baseExecutionClassName);
            string className = "whileActivityConditionExecutionContext";
            nmSpaceCodeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSNAME#", baseExecutionClassName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSCODE#", baseExecutionClassCode);
            nmSpaceCodeBuilder.Replace("#CLASSNAME#", className);
            nmSpaceCodeBuilder.Replace("#CONDITION#", this.Condition.GetCode(null));

            var insertNewIterationVisualEventInput = new GenerateInsertVisualEventCodeInput
            {
                ActivityContextVariableName = "_activityContext",
                ActivityId = context.VRWorkflowActivityId,
                EventTitle = @"""While Activity New Iteration""",
                EventTypeId = CodeGenerationHelper.VISUALEVENTTYPE_NEWITERATION
            };
            nmSpaceCodeBuilder.Replace("#INSERTNEWITERATIONVISUALEVENTCODE#",
                context.GenerateInsertVisualEventCode(insertNewIterationVisualEventInput));

            var insertCompletedVisualEventInput = new GenerateInsertVisualEventCodeInput
            {
                ActivityContextVariableName = "_activityContext",
                ActivityId = context.VRWorkflowActivityId,
                EventTitle = $@"""While Activity completed""",
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
                                new System.Activities.Statements.While((activityContext) => new #NAMESPACE#.#CLASSNAME#(activityContext).Evaluate())
                                {
                                    Body = new Sequence
                                            {
                                                Activities =
                                                {
                                                    new #NAMESPACE#.#CLASSNAME#_InsertNewIterationVisualEventActivity(),
                                                    #ACTIVITYCODE#
                                                }
                                            }
                                },
                                new #NAMESPACE#.#CLASSNAME#_InsertCompletedVisualEventActivity()
                            }
                        }");

            codeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            codeBuilder.Replace("#CLASSNAME#", className);

            var activityContext = context.CreateChildContext(context.GenerateInsertVisualEventCode);
            activityContext.VRWorkflowActivityId = this.Activity.VRWorkflowActivityId;
            codeBuilder.Replace("#ACTIVITYCODE#", this.Activity.Settings.GenerateWFActivityCode(activityContext));

            return codeBuilder.ToString();
        }

        public override BPVisualItemDefinition GetVisualItemDefinition(IVRWorkflowActivityGetVisualItemDefinitionContext context)
        {
            if (this.Activity != null && this.Activity.Settings != null)
            {
                var childActivityVisualItem = this.Activity.Settings.GetVisualItemDefinition(context);
                if(childActivityVisualItem != null)
                {
                    return new BPVisualItemDefinition
                    {
                        Settings = new BPWhileVisualItemDefinition
                        {
                            ConditionDescription = !String.IsNullOrEmpty(this.ConditionDescription) ? this.ConditionDescription : this.Condition.GetCode(null),
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

    public class BPWhileVisualItemDefinition : BPVisualItemDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("22BB8BB3-5DA3-4200-911C-E8831CA7C513"); } }

        public override string Editor { get { return "bp-workflow-activitysettings-visualitemdefiniton-while"; } }

        public string ConditionDescription { get; set; }

        public Guid ChildActivityId { get; set; }

        public BPVisualItemDefinition ChildActivityVisualItemDefinition { get; set; }
    }
}
