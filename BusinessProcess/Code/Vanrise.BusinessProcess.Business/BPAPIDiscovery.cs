using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPAPIDiscovery : VRAPIDiscovery
    {
        public override bool IsCacheExpired(ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<Vanrise.BusinessProcess.Business.BPDefinitionManager.BPDefinitionWorkflowCacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override System.Collections.Generic.List<string> GetModuleNames(IVRAPIDiscoveryGetModuleNamesContext context)
        {
            return new System.Collections.Generic.List<string> { "DynamicBusinessProcess_BP" };
        }

        public override List<Type> GetControllerTypes(IVRAPIDiscoveryGetControllerTypesContext context)
        {
            List<Type> controllerTypes = new List<Type>();

            IEnumerable<BPDefinition> bpDefinitions = new BPDefinitionManager().GetBPDefinitions();
            foreach (var bpDefinition in bpDefinitions)
            {
                if (!bpDefinition.VRWorkflowId.HasValue)
                    continue;

                controllerTypes.Add(GetBPControllerType(bpDefinition));
            }

            return controllerTypes;
        }

        private Type GetBPControllerType(BPDefinition bpDefinition)
        {
            StringBuilder sb_CustomController = new StringBuilder(@"using System;
                        using System.Collections.Generic;
                        using System.Web.Http;
                        using Vanrise.Web.Base;
                        using Vanrise.BusinessProcess.Entities;
                        using Vanrise.BusinessProcess.Business;
        
                        namespace #Namespace#
                        {
                            [RoutePrefix(""api/DynamicBusinessProcess_BP/#ControllerName#"")]
                            public class #ControllerFullName# : BaseAPIController
                            {
                                [HttpPost]
                                [Route(""#StartProcess#"")]
                                public void #StartProcess#(#StartProcessInputClassName# input)
                                {
                                    if(input == null)
                                        input = new #StartProcessInputClassName#();

                                    input.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
                                    CreateProcessOutput createProcessOutput = new BPInstanceManager().CreateNewProcess(new CreateProcessInput() { InputArguments = input }, true);
                                }
                            }
        
                            #StartProcessInputClass#
                        }");

            BPDefinitionManager bpDefinitionManager = new BPDefinitionManager();

            string startProcessInputClassName;
            string startProcessInputClass = Helper.GetInputArgumentClassCode(bpDefinition, out startProcessInputClassName);

            string bpDefinitionTitle = bpDefinition.Title.Replace(" ", "");
            string controllerName = bpDefinitionTitle;
            string controllerFullName = string.Concat(controllerName, "Controller");
            string controllerNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.BusinessProcess.Web.DynamicControllers");
            string startProcessFunctionName = string.Concat("Start", bpDefinitionTitle, "Process");

            sb_CustomController.Replace("#Namespace#", controllerNamespace);
            sb_CustomController.Replace("#ControllerName#", controllerName);
            sb_CustomController.Replace("#ControllerFullName#", controllerFullName);
            sb_CustomController.Replace("#StartProcess#", startProcessFunctionName);
            sb_CustomController.Replace("#StartProcessInputClass#", startProcessInputClass);
            sb_CustomController.Replace("#StartProcessInputClassName#", startProcessInputClassName);

            Common.CSharpCompilationOutput output;
            Common.CSharpCompiler.TryCompileClass(sb_CustomController.ToString(), out output);
            return output.OutputAssembly.GetType(string.Format("{0}.{1}", controllerNamespace, controllerFullName));
        }
    }
}