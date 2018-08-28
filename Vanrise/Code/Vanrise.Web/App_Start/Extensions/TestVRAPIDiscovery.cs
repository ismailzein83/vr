using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Entities;

namespace Vanrise.Web.App_Start.Extensions
{
    public class TestVRAPIDiscovery : VRAPIDiscovery
    {
        public override System.Collections.Generic.List<Type> GetControllerTypes(IVRAPIDiscoveryGetControllerTypesContext context)
        {
            return new List<Type> { CompileControllerType() };
        }

        static int NbOfGenerationTime;

        private static Type CompileControllerType()
        {
            string customController = @"using System;
                using System.Collections.Generic;
                using System.Web.Http;
                using Vanrise.Web.Base;

                namespace Vanrise.DynamicControllers_#NamespaceName#
                {
                    [RoutePrefix(""api/DynamicModule/VRDynGen"")]
                    public class VRDynGenController : BaseAPIController
                    {
                        //[Vanrise.Entities.IsAnonymous]
                        [HttpGet]
                        [Route(""TestCall"")]
                        public string TestCall(string input)
                        {
                            return String.Concat(input, "" from DynGen Controller using VRHttpControllerSelector V3:"", DateTime.Now, ""\nController generated at: #GENERATIONTIME#"", ""\n #NBOFGENERATIONTIME# times generated"");
                        }
                    }
                }";

            NbOfGenerationTime++;
            string namespaceName = Guid.NewGuid().ToString().Replace("-", "");
            Common.CSharpCompilationOutput output;
            Common.CSharpCompiler.TryCompileClass(customController.Replace("#NamespaceName#", namespaceName).Replace("#GENERATIONTIME#", DateTime.Now.ToString()).Replace("#NBOFGENERATIONTIME#", NbOfGenerationTime.ToString()), out output);
            return output.OutputAssembly.GetType("Vanrise.DynamicControllers_" + namespaceName + ".VRDynGenController");
        }

        public override bool IsCacheExpired(ref DateTime LastCheckTime)
        {
            if ((DateTime.Now - LastCheckTime).TotalSeconds >= 20)
            {
                LastCheckTime = DateTime.Now;
                return true;
            }
            else
                return false;
        }

        public override System.Collections.Generic.List<string> GetModuleNames(IVRAPIDiscoveryGetModuleNamesContext context)
        {
            return new System.Collections.Generic.List<string> { "DynamicModule" };
        }
    }
}