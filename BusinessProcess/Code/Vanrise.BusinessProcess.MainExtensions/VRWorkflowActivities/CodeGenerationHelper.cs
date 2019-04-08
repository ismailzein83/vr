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
        public static Guid VISUALEVENTTYPE_STARTED = new Guid("372F4B89-68FF-4C47-8CD5-E501A71C46F7");
        public static Guid VISUALEVENTTYPE_COMPLETED = new Guid("DEAAE89F-E3C9-41ED-BC84-0BD8A58D0A32");
        public static Guid VISUALEVENTTYPE_ERROR = new Guid("F3A80E84-87F1-4073-9D9F-78C394AA0495");
        public static Guid VISUALEVENTTYPE_RETRYING = new Guid("AD5A31E5-2890-4EF5-9339-61F86B536CFE");

        public static string GenerateBaseExecutionClass(IVRWorkflowActivityGenerateWFActivityCodeContext context, string className)
        {
            StringBuilder codeBuilder = new StringBuilder(@"                 
                    public class #CLASSNAME#
                    {                        
                        System.Activities.WorkflowDataContext _workflowDataContext;
                        System.Activities.ActivityContext _activityContext;

                        public #CLASSNAME#(ActivityContext activityContext)
                        {
                            _activityContext = activityContext;
                            _workflowDataContext = activityContext.DataContext;
                        }

                        public void WriteWarning(string message)
                        {
                            _activityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Warning, message);
                        }

                        public void WriteInformation(string message)
                        {
                            _activityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message);
                        }

                        public void WriteVerbose(string message)
                        {
                            _activityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Verbose, message);
                        }

                        public void WriteError(string message)
                        {
                            _activityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Error, message);
                        }

                        public void WriteHandledException(Exception ex)
                        {
                            _activityContext.WriteHandledException(ex);
                        }

                        public long GetProcessInstanceId()
                        {
                            return _activityContext.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
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

                ReplaceVariablePlaceHolders(variablePropertyCodeBuilder, wfVariable.Name, wfVariable.Type);
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
                ReplaceVariablePlaceHolders(variablePropertyCodeBuilder, wfArgument.Name, wfArgument.Type);
                variablesPropertiesCodeBuilder.AppendLine(variablePropertyCodeBuilder.ToString());
            }

            return variablesPropertiesCodeBuilder.ToString();
        }

        private static void ReplaceVariablePlaceHolders(StringBuilder variablePropertyCodeBuilder, string variableName, VRWorkflowVariableType variableType)
        {
            variablePropertyCodeBuilder.Replace("#VARIABLENAME#", variableName);
            Type wfVariableRuntimeType = variableType.GetRuntimeType(null);
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
        }
    }
}