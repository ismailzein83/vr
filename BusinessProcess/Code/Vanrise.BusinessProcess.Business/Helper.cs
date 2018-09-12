using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Business
{
    public static class Helper
    {
        public static Type GetInputArgumentType(BPDefinition bpDefinition)
        {
            StringBuilder sb_CustomController = new StringBuilder(@"using System;
                        using System.Collections.Generic;
                        using System.Linq;
                        using Vanrise.Common;
                        using Vanrise.BusinessProcess.Entities;
        
                        namespace #Namespace#
                        {       
                            #InputArgumentClass#
                        }");


            string inputArgumentClassNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.BusinessProcess.Entities");
            sb_CustomController.Replace("#Namespace#", inputArgumentClassNamespace);

            string inputArgumentClassName;
            string inputArgumentClassCode = GetInputArgumentClassCode(bpDefinition, out inputArgumentClassName);
            sb_CustomController.Replace("#InputArgumentClass#", inputArgumentClassCode);

            Common.CSharpCompilationOutput output;
            if (!Common.CSharpCompiler.TryCompileClass(sb_CustomController.ToString(), out output))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                foreach (var errorMessage in output.ErrorMessages)
                    errorsBuilder.AppendLine(errorMessage);

                throw new Exception(String.Format("Compile Error when building VRWorkflowInputArgument for BPDefinition Id '{0}'. Errors: {1}",
                    bpDefinition.BPDefinitionID, errorsBuilder));
            }

            return output.OutputAssembly.GetType(string.Format("{0}.{1}", inputArgumentClassNamespace, inputArgumentClassName));
        }

        public static string GetInputArgumentClassCode(BPDefinition bpDefinition, out string inputArgumentClassName)
        {
            StringBuilder sb_InputArgumentClassCode = new StringBuilder(@"
                            public class #InputArgumentClassName# : VRWorkflowInputArgument
                            {
                                Guid s_bpDefinitionId = new Guid(""#BPDefinitionId#"");

                                #InputArgumentClassProperties#
        
                                public override string ProcessName { get { return string.Format(""VRWorkflowInputArgument_{0}"", s_bpDefinitionId.ToString(""N"")); } }
        
                                public override string GetTitle()
                                {
                                    return #GetTitleExecutionCode#;
                                }

                                public override Dictionary<string, object> ConvertArgumentsToDictionary()
                                {
                                    Dictionary<string, object> arguments = new Dictionary<string, object>();
                                    #InputArgumentDictionaryBuilderCode#
                                    return arguments;
                                }
                            }");

            Guid vrWorkflowId = bpDefinition.VRWorkflowId.Value;
            VRWorkflow vrWorkflow = new VRWorkflowManager().GetVRWorkflow(vrWorkflowId);
            vrWorkflow.ThrowIfNull("vrWorkflow", vrWorkflowId);
            vrWorkflow.Settings.ThrowIfNull("vrWorkflow.Settings", vrWorkflowId);

            StringBuilder sb_InputArgumentClassPropertiesBuilder = new StringBuilder();
            StringBuilder sb_InputArgumentDictionaryBuilder = new StringBuilder();

            if (vrWorkflow.Settings.Arguments != null)
            {
                foreach (var argument in vrWorkflow.Settings.Arguments)
                {
                    switch (argument.Direction)
                    {
                        case VRWorkflowArgumentDirection.In:
                        case VRWorkflowArgumentDirection.InOut:
                            string fieldRuntimeTypeAsString = CSharpCompiler.TypeToString(argument.Type.GetRuntimeType(null));

                            sb_InputArgumentClassPropertiesBuilder.Append(GetProperty(fieldRuntimeTypeAsString, argument.Name));
                            sb_InputArgumentClassPropertiesBuilder.AppendLine();

                            sb_InputArgumentDictionaryBuilder.AppendFormat(@"arguments.Add(""{0}"", {0});", argument.Name);
                            sb_InputArgumentDictionaryBuilder.AppendLine();
                            break;

                        case VRWorkflowArgumentDirection.Out:
                            break;

                        default: throw new NotSupportedException(String.Format("wfArgument.Direction '{0}'", argument.Direction.ToString()));
                    }
                }
            }

            string getTitleExecutionCode;
            if (bpDefinition.Configuration != null && !string.IsNullOrEmpty(bpDefinition.Configuration.ProcessTitle))
                getTitleExecutionCode = bpDefinition.Configuration.ProcessTitle;
            else
                getTitleExecutionCode = string.Concat("\"", vrWorkflow.Title, "\"");

            string bpDefinitionTitle = bpDefinition.Title.Replace(" ", "");
            inputArgumentClassName = string.Concat(bpDefinitionTitle, "InputArgument");

            string inputArgumentClassPropertiesBuilder = sb_InputArgumentClassPropertiesBuilder.Length > 0 ? sb_InputArgumentClassPropertiesBuilder.ToString() : string.Empty;
            string inputArgumentDictionaryBuilder = sb_InputArgumentDictionaryBuilder.Length > 0 ? sb_InputArgumentDictionaryBuilder.ToString() : string.Empty;

            sb_InputArgumentClassCode.Replace("#BPDefinitionId#", bpDefinition.BPDefinitionID.ToString());
            sb_InputArgumentClassCode.Replace("#InputArgumentClassName#", inputArgumentClassName);
            sb_InputArgumentClassCode.Replace("#InputArgumentClassProperties#", inputArgumentClassPropertiesBuilder);
            sb_InputArgumentClassCode.Replace("#InputArgumentDictionaryBuilderCode#", inputArgumentDictionaryBuilder);
            sb_InputArgumentClassCode.Replace("#GetTitleExecutionCode#", getTitleExecutionCode);

            return sb_InputArgumentClassCode.ToString();
        }

        private static string GetProperty(string fieldRuntimeTypeAsString, string fieldName)
        {
            StringBuilder propertiesBuilder = new StringBuilder();
            propertiesBuilder.Append(@"public #FieldRuntimeType# #FieldName# { get; set; }");
            propertiesBuilder.Replace("#FieldRuntimeType#", fieldRuntimeTypeAsString);
            propertiesBuilder.Replace("#FieldName#", fieldName);
            return propertiesBuilder.ToString();
        }
    }
}
