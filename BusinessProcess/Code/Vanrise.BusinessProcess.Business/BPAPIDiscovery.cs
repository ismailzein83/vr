using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPAPIDiscovery //: VRAPIDiscovery
    {
        //        public override List<Type> GetControllerTypes(IVRAPIDiscoveryGetControllerTypesContext context)
        //        {
        //            List<Type> controllerTypes = new List<Type>();

        //            IEnumerable<BPDefinition> bpDefinitions = new BPDefinitionManager().GetBPDefinitions();
        //            foreach (var bpDefinition in bpDefinitions)
        //            {
        //                if (!bpDefinition.VRWorkflowId.HasValue)
        //                    continue;

        //                controllerTypes.Add(CompileBPControllerType(bpDefinition.BPDefinitionID, bpDefinition.Title, bpDefinition.VRWorkflowId.Value));
        //            }

        //            return controllerTypes;
        //        }


        //        private Type CompileBPControllerType(Guid bpDefinitionID, string bpDefinitionTitle, Guid vrWorkflowId)
        //        {
        //            StringBuilder sb_CustomController = new StringBuilder(@"using System;
        //                using System.Collections.Generic;
        //                using System.Web.Http;
        //                using Vanrise.Web.Base;
        //                using Vanrise.BusinessProcess.Entities;
        //                using Vanrise.BusinessProcess.Business;
        //
        //                namespace #Namespace#
        //                {
        //                    [RoutePrefix(""api/BusinessProcess/#ControllerName#"")]
        //                    public class #ControllerName#Controller : BaseAPIController
        //                    {
        //                        [HttpPost]
        //                        [Route(""#StartProcess#"")]
        //                        public #StartProcessOutputClassName# #StartProcess#(#StartProcessInputClassName# input)
        //                        {
        //                            var inputArgument = this.GetVRWorkflowInputArgument(input);
        //                            inputArgument.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
        //                            var createProcessInput = new CreateProcessInput() { InputArguments = inputArgument };
        //
        //                            return new BPInstanceManager().CreateNewProcess(createProcessInput, true);
        //                        }
        //
        //                        private VRWorkflowInputArgument GetVRWorkflowInputArgument(#StartProcessInputClassName# input)
        //                        {
        //                            List<string> argumentNames = new List<string>() { #ArgumentNames# };
        //                            Dictionary<string, object> arguments = new Dictionary<string, object>();
        //
        //                            foreach(var argumentName in argumentNames)
        //                            {
        //                                if(!results.ContainsKey(argumentName))
        //                                    arguments.Add(argumentName, input.GetFieldValue(argumentName));
        //                            }
        //
        //                            return new VRWorkflowInputArgument() { BPDefinitionId = new Guid(""#BPDefinitionId#""), Arguments = arguments };
        //                        }
        //                    }
        //
        //                    public class #StartProcessInputClassName#
        //                    {
        //                        #StartProcessInputClassProperties#
        //
        //                        private static Dictionary<string, Func<#Namespace#.#StartProcessInputClassName#, object>> s_getFieldValueActions;
        //
        //                        static #StartProcessInputClassName#()
        //                        {
        //                             BuildGetFieldValueActions();
        //                        }
        //
        //                        public dynamic GetFieldValue(string fieldName)
        //                        {
        //                            Func<#Namespace#.#StartProcessInputClassName#, dynamic> getFieldValueAction;
        //                            if(!s_getFieldValueActions.TryGetValue(fieldName, out getFieldValueAction))
        //                                throw new ArgumentException(String.Format(""argumentName '{0}'"", fieldName), ""fieldName"");
        //                            return getFieldValueAction(this);
        //                        }
        //
        //                        private static void BuildGetFieldValueActions()
        //                        {
        //                            s_getFieldValueActions = new Dictionary<string, Func<#Namespace#.#StartProcessInputClassName#, object>>(#FieldsCount#);
        //                            #BuildGetFieldValueActions#
        //                        }
        //                    }
        //
        //                    #StartProcessOutputClass#
        //                }");

        //            VRWorkflow vrWorkflow = new VRWorkflowManager().GetVRWorkflow(vrWorkflowId);
        //            vrWorkflow.ThrowIfNull("vrWorkflow", vrWorkflowId);
        //            vrWorkflow.Settings.ThrowIfNull("vrWorkflow.Settings", vrWorkflowId);
        //            vrWorkflow.Settings.Arguments.ThrowIfNull("vrWorkflow.Settings.Arguments", vrWorkflowId);

        //            StringBuilder sb_GetFieldValueActionsBuilder = new StringBuilder();
        //            StringBuilder sb_StartProcessInputClassProperties = new StringBuilder();
        //            StringBuilder sb_StartProcessOutputClassProperties = new StringBuilder();

        //            foreach (var argument in vrWorkflow.Settings.Arguments)
        //            {
        //                sb_GetFieldValueActionsBuilder.AppendFormat(@"s_getFieldValueActions.Add(""{0}"", (startProcessInput) => startProcessInput.{0});", argument.Name);
        //                sb_GetFieldValueActionsBuilder.AppendLine();

        //                string fieldRuntimeTypeAsString = CSharpCompiler.TypeToString(argument.Type.GetRuntimeType(null));

        //                switch (argument.Direction)
        //                {
        //                    case VRWorkflowArgumentDirection.In:
        //                        sb_StartProcessInputClassProperties.Append(this.GetProperty(fieldRuntimeTypeAsString, argument.Name));
        //                        sb_StartProcessInputClassProperties.AppendLine();
        //                        break;

        //                    case VRWorkflowArgumentDirection.InOut:
        //                        sb_StartProcessInputClassProperties.Append(this.GetProperty(fieldRuntimeTypeAsString, argument.Name));
        //                        sb_StartProcessInputClassProperties.AppendLine();

        //                        sb_StartProcessOutputClassProperties.Append(this.GetProperty(fieldRuntimeTypeAsString, argument.Name));
        //                        sb_StartProcessOutputClassProperties.AppendLine();
        //                        break;

        //                    case VRWorkflowArgumentDirection.Out:
        //                        sb_StartProcessOutputClassProperties.Append(this.GetProperty(fieldRuntimeTypeAsString, argument.Name));
        //                        sb_StartProcessOutputClassProperties.AppendLine();
        //                        break;

        //                    default: throw new NotSupportedException(String.Format("wfArgument.Direction '{0}'", argument.Direction.ToString()));
        //                }
        //            }

        //            bpDefinitionTitle = bpDefinitionTitle.Replace(" ", "");
        //            string controllerNamespace = "Vanrise.BusinessProcess.Web.DynamicControllers"; //string.Format("Vanrise.BusinessProcess.Web.DynamicControllers_{0}", Guid.NewGuid().ToString().Replace("-", ""));
        //            string startProcessFunctionName = string.Concat("Start", bpDefinitionTitle, "Process");
        //            string startProcessInputClassName = string.Concat(startProcessFunctionName, "Input");

        //            string startProcessOutputClassName; 
        //            string startProcessOutputClass;
        //            if (sb_StartProcessOutputClassProperties.Length > 0)
        //            {
        //                StringBuilder sb_StartProcessOutputClass = new StringBuilder(@"public class #StartProcessOutputClassName#
        //                    {
        //                        #StartProcessOutputClassProperties#
        //                    }");

        //                startProcessOutputClassName = string.Concat(startProcessFunctionName, "Output");
        //                sb_StartProcessOutputClass.Replace("#StartProcessOutputClassName#", startProcessOutputClassName);
        //                sb_StartProcessOutputClass.Replace("#StartProcessOutputClassProperties#", sb_StartProcessOutputClassProperties.ToString());
        //                startProcessOutputClass = sb_StartProcessOutputClass.ToString();
        //            }
        //            else
        //            {
        //                startProcessOutputClassName = "void";
        //                startProcessOutputClass = null;
        //            }

        //            string controllerName = bpDefinitionTitle;
        //            sb_CustomController.Replace("#Namespace#", controllerNamespace);
        //            sb_CustomController.Replace("#ControllerName#", controllerName);
        //            sb_CustomController.Replace("#BPDefinitionId#", bpDefinitionID.ToString());
        //            sb_CustomController.Replace("#FieldsCount#", vrWorkflow.Settings.Arguments.Count.ToString());
        //            sb_CustomController.Replace("#ArgumentNames#", string.Join(",", vrWorkflow.Settings.Arguments.Select(itm => itm.Name)));
        //            sb_CustomController.Replace("#StartProcess#", startProcessFunctionName);
        //            sb_CustomController.Replace("#StartProcessInputClassName#", startProcessInputClassName);
        //            sb_CustomController.Replace("#StartProcessInputClassProperties#", sb_StartProcessInputClassProperties.ToString());
        //            sb_CustomController.Replace("#StartProcessOutputClassName#", startProcessOutputClassName);
        //            sb_CustomController.Replace("#StartProcessOutputClass#", !string.IsNullOrEmpty(startProcessOutputClass) ? startProcessOutputClass : string.Empty);
        //            sb_CustomController.Replace("#BuildGetFieldValueActions#", sb_GetFieldValueActionsBuilder.ToString());

        //            Common.CSharpCompilationOutput output;
        //            Common.CSharpCompiler.TryCompileClass(sb_CustomController.ToString(), out output);
        //            return output.OutputAssembly.GetType(string.Format("{0}.{1}", controllerNamespace, controllerName));
        //        }

        //        private string GetProperty(string fieldRuntimeTypeAsString, string fieldName)
        //        {
        //            StringBuilder propertiesBuilder = new StringBuilder();
        //            propertiesBuilder.Append(@"public #FieldRuntimeType# #FieldName# { get; set; }");
        //            propertiesBuilder.Replace("#FieldRuntimeType#", fieldRuntimeTypeAsString);
        //            propertiesBuilder.Replace("#FieldName#", fieldName);
        //            return propertiesBuilder.ToString();
        //        }

        //        DateTime? _bpDefinitionCacheLastCheck;
        //        DateTime? _vrWorkflowCacheLastCheck;
        //        public override bool IsCacheExpired(ref DateTime LastCheckTime)
        //        {
        //            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<BPDefinitionManager.CacheManager>().IsCacheExpired(ref _bpDefinitionCacheLastCheck)
        //                   | Vanrise.Caching.CacheManagerFactory.GetCacheManager<VRWorkflowManager.CacheManager>().IsCacheExpired(ref _vrWorkflowCacheLastCheck);
        //        }

        //        public override System.Collections.Generic.List<string> GetModuleNames(IVRAPIDiscoveryGetModuleNamesContext context)
        //        {
        //            return new System.Collections.Generic.List<string> { "BusinessProcess_BP" };
        //        }
    }
}