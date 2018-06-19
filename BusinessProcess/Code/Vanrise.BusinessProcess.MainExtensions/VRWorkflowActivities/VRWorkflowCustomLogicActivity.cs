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
            get { throw new NotImplementedException(); }
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

            //string variablesPropertiesCode = BuildVariablesPropertiesCode(context);
            //nmSpaceCodeBuilder.Replace("#VARIABLESPROPERTIES#", variablesPropertiesCode);

            //string argumentsPropertiesCode = BuildArgumentsPropertiesCode(context);
            //nmSpaceCodeBuilder.Replace("#ARGUMENTSPROPERTIES#", argumentsPropertiesCode);

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

        private string BuildVariablesPropertiesCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder variablesPropertiesCodeBuilder = new StringBuilder();
            foreach (var wfVariable in context.GetAllVariables())
            {
                StringBuilder variablePropertyCodeBuilder = new StringBuilder(@"

System.ComponentModel.PropertyDescriptor _#VARIABLENAME#PropertyDescriptor;
public #VARIABLETYPE# #VARIABLENAME#
{
    get
    {
        if(_#VARIABLENAME#PropertyDescriptor == null)
        {
            _#VARIABLENAME#PropertyDescriptor = _workflowDataContext.GetProperties()[""#VARIABLENAME#""];
            _#VARIABLENAME#PropertyDescriptor.ThrowIfNull(""_#VARIABLENAME#PropertyDescriptor"");
        }
        return #VARIABLEVALUETYPECAST#_#VARIABLENAME#PropertyDescriptor.GetValue(_workflowDataContext)#VARIABLEREFERENCETYPECAST#;
    }
    set
    {
        if(_#VARIABLENAME#PropertyDescriptor == null)
        {
            _#VARIABLENAME#PropertyDescriptor = _workflowDataContext.GetProperties()[""#VARIABLENAME#""];
            _#VARIABLENAME#PropertyDescriptor.ThrowIfNull(""_#VARIABLENAME#PropertyDescriptor"");
        }
        _#VARIABLENAME#PropertyDescriptor.SetValue(_workflowDataContext, value);

    }
}

");
                
                variablePropertyCodeBuilder.Replace("#VARIABLENAME#", wfVariable.Name);
                Type wfVariableRuntimeType = wfVariable.Type.GetRuntimeType(null);
                string wfVariableRuntimeTypeAsString = CSharpCompiler.TypeToString(wfVariableRuntimeType);
                variablePropertyCodeBuilder.Replace("#VARIABLETYPE#", wfVariableRuntimeTypeAsString);
                if (wfVariableRuntimeType.IsValueType)
                {
                    variablePropertyCodeBuilder.Replace("#VARIABLEVALUETYPECAST#", string.Concat("(", wfVariableRuntimeTypeAsString, ")"));
                    variablePropertyCodeBuilder.Replace("#VARIABLEREFERENCETYPECAST#", "");
                }
                else
                {
                    variablePropertyCodeBuilder.Replace("#VARIABLEREFERENCETYPECAST#", string.Concat(" as ", wfVariableRuntimeTypeAsString));
                    variablePropertyCodeBuilder.Replace("#VARIABLEVALUETYPECAST#", "");
                }
                variablesPropertiesCodeBuilder.AppendLine(variablePropertyCodeBuilder.ToString());
            }

            return variablesPropertiesCodeBuilder.ToString();
        }

        private string BuildArgumentsPropertiesCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder argumentsPropertiesCodeBuilder = new StringBuilder();

            string propertyGetter = @"
get
    {
        if(_#VARIABLENAME#PropertyDescriptor == null)
        {
            _#VARIABLENAME#PropertyDescriptor = _workflowDataContext.GetProperties()[""#VARIABLENAME#""];
            _#VARIABLENAME#PropertyDescriptor.ThrowIfNull(""_#VARIABLENAME#PropertyDescriptor"");
        }
        return #VARIABLEVALUETYPECAST#_#VARIABLENAME#PropertyDescriptor.GetValue(_workflowDataContext)#VARIABLEREFERENCETYPECAST#;
    }
";

            string propertySetter = @"
set
    {
        if(_#VARIABLENAME#PropertyDescriptor == null)
        {
            _#VARIABLENAME#PropertyDescriptor = _workflowDataContext.GetProperties()[""#VARIABLENAME#""];
            _#VARIABLENAME#PropertyDescriptor.ThrowIfNull(""_#VARIABLENAME#PropertyDescriptor"");
        }
        _#VARIABLENAME#PropertyDescriptor.SetValue(_workflowDataContext, value);

    }
";

            foreach (var wfArgument in context.GetAllWorkflowArguments())
            {
                StringBuilder variablePropertyCodeBuilder = new StringBuilder(@"

System.ComponentModel.PropertyDescriptor _#VARIABLENAME#PropertyDescriptor;
public #VARIABLETYPE# #VARIABLENAME#
{
#PROPERTYGETTER#
#PROPERTYSETTER#
}

");

                switch(wfArgument.Direction)
                {
                    case VRWorkflowArgumentDirection.In :
                        variablePropertyCodeBuilder.Replace("#PROPERTYGETTER#", propertyGetter);
                        variablePropertyCodeBuilder.Replace("#PROPERTYSETTER#", "");
                        break;
                    case VRWorkflowArgumentDirection.Out:
                        variablePropertyCodeBuilder.Replace("#PROPERTYGETTER#", "");
                        variablePropertyCodeBuilder.Replace("#PROPERTYSETTER#", propertySetter);
                        break;
                    case VRWorkflowArgumentDirection.InOut:
                        variablePropertyCodeBuilder.Replace("#PROPERTYGETTER#", propertyGetter);
                        variablePropertyCodeBuilder.Replace("#PROPERTYSETTER#", propertySetter);
                        break;
                    default: throw new NotSupportedException(String.Format("wfArgument.Direction '{0}'", wfArgument.Direction.ToString()));
                }

                variablePropertyCodeBuilder.Replace("#VARIABLENAME#", wfArgument.Name);
                Type wfVariableRuntimeType = wfArgument.Type.GetRuntimeType(null);
                string wfVariableRuntimeTypeAsString = CSharpCompiler.TypeToString(wfVariableRuntimeType);
                variablePropertyCodeBuilder.Replace("#VARIABLETYPE#", wfVariableRuntimeTypeAsString);
                if (wfVariableRuntimeType.IsValueType)
                {
                    variablePropertyCodeBuilder.Replace("#VARIABLEVALUETYPECAST#", string.Concat("(", wfVariableRuntimeTypeAsString, ")"));
                    variablePropertyCodeBuilder.Replace("#VARIABLEREFERENCETYPECAST#", "");
                }
                else
                {
                    variablePropertyCodeBuilder.Replace("#VARIABLEREFERENCETYPECAST#", string.Concat(" as ", wfVariableRuntimeTypeAsString));
                    variablePropertyCodeBuilder.Replace("#VARIABLEVALUETYPECAST#", "");
                }
                argumentsPropertiesCodeBuilder.AppendLine(variablePropertyCodeBuilder.ToString());
            }

            return argumentsPropertiesCodeBuilder.ToString();
        }
    }

    public class VRWorkflowCustomLogicActivityExecutionContext : IVRWorkflowCustomLogicActivityExecutionContext
    {
        System.Activities.CodeActivityContext _activityContext;
        public VRWorkflowCustomLogicActivityExecutionContext(System.Activities.CodeActivityContext activityContext)
        {
            _activityContext = activityContext;
        }
    }

    public interface IVRWorkflowCustomLogicActivityExecutionContext
    {
        
    }
}
