using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public static class CodeGenerationHelper
    {
        public static string GenerateBaseExecutionClass(IVRWorkflowActivityGenerateWFActivityCodeContext context, string className)
        {
            StringBuilder codeBuilder = new StringBuilder(@"                 

                
                    public class #CLASSNAME#
                    {                        
                        System.Activities.WorkflowDataContext _workflowDataContext;

                        public #CLASSNAME#(ActivityContext activityContext)
                        {
                            _workflowDataContext = activityContext.DataContext;
                        }

                        #VARIABLESPROPERTIES#
                    }

                ");

            codeBuilder.Replace("#VARIABLESPROPERTIES#", BuildVariablesPropertiesCode(context));
            codeBuilder.Replace("#CLASSNAME#", className);

            return codeBuilder.ToString();
        }


        private static string BuildVariablesPropertiesCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder variablesPropertiesCodeBuilder = new StringBuilder();
            
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

            string variablePropertyTemplate = @"

System.ComponentModel.PropertyDescriptor _#VARIABLENAME#PropertyDescriptor;
public #VARIABLETYPE# #VARIABLENAME#
{
#PROPERTYGETTER#
#PROPERTYSETTER#
}

";
            foreach (var wfVariable in context.GetAllVariables())
            {
                StringBuilder variablePropertyCodeBuilder = new StringBuilder(variablePropertyTemplate);
               
                        variablePropertyCodeBuilder.Replace("#PROPERTYGETTER#", propertyGetter);
                        variablePropertyCodeBuilder.Replace("#PROPERTYSETTER#", propertySetter);
                       

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

            foreach (var wfArgument in context.GetAllWorkflowArguments())
            {
                StringBuilder variablePropertyCodeBuilder = new StringBuilder(variablePropertyTemplate);

                switch (wfArgument.Direction)
                {
                    case VRWorkflowArgumentDirection.In:
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
                variablesPropertiesCodeBuilder.AppendLine(variablePropertyCodeBuilder.ToString());
            }

            return variablesPropertiesCodeBuilder.ToString();
        }
    }
}
