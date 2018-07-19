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
        public override Guid ConfigId
        {
            get { return new Guid("BA3A107D-20DA-4456-AC70-A2948DFE3725"); }
        }

		public override string Editor
		{
			get { return "vr-workflow-foreach"; }
		}

		public override string Title
		{
			get { return "ForEach"; }
		}

		public string List { get; set; }

        public string IterationVariableName { get; set; }

        public VRWorkflowVariableType IterationVariableType { get; set; }

        public VRWorkflowActivity Activity { get; set; }

        public override string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
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

            var childContext = context.CreateChildContext();
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
            nmSpaceCodeBuilder.Replace("#LISTNAME#", this.List);
            nmSpaceCodeBuilder.Replace("#ITERATIONVARIABLERUNTIMETYPE#", iterationVariableRuntimeType);
            

            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.Append(@"
            new System.Activities.Statements.ForEach<#ITERATIONVARIABLERUNTIMETYPE#>
            {
                Values =  new System.Activities.InArgument<IEnumerable<#ITERATIONVARIABLERUNTIMETYPE#>>((activityContext) => new #NAMESPACE#.#CLASSNAME#(activityContext).List),
                Body = new System.Activities.ActivityAction<#ITERATIONVARIABLERUNTIMETYPE#>
                                {
                                     Argument = new System.Activities.DelegateInArgument<#ITERATIONVARIABLERUNTIMETYPE#>(""#ITERATIONVARIABLENAME#""),
                                     Handler = #ACTIVITYCODE#                      
                                }
            }");

            codeBuilder.Replace("#NAMESPACE#", nmSpaceName);
            codeBuilder.Replace("#CLASSNAME#", className);
            codeBuilder.Replace("#ITERATIONVARIABLERUNTIMETYPE#", iterationVariableRuntimeType);
            codeBuilder.Replace("#ITERATIONVARIABLENAME#", this.IterationVariableName);


            codeBuilder.Replace("#ACTIVITYCODE#", this.Activity.Settings.GenerateWFActivityCode(childContext));
            return codeBuilder.ToString();
        }
    }
}
