using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.Common.Business
{

    public class VRDynamicAPIDiscovery : VRAPIDiscovery
    {

        public override System.Collections.Generic.List<Type> GetControllerTypes(IVRAPIDiscoveryGetControllerTypesContext context)
        {
            return CompileControllerType();
        }
        private static List<Type> CompileControllerType()
        {
            Vanrise.Common.Business.VRDynamicAPIManager vrDynamicAPIManager = new Vanrise.Common.Business.VRDynamicAPIManager();
            return vrDynamicAPIManager.BuildAllDynamicAPIControllers();
        }

        public override bool IsCacheExpired(ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<Vanrise.Common.Business.VRDynamicAPIManager.CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override System.Collections.Generic.List<string> GetModuleNames(IVRAPIDiscoveryGetModuleNamesContext context)
        {
            Vanrise.Common.Business.VRDynamicAPIModuleManager vrDynamicAPIModuleManager = new Vanrise.Common.Business.VRDynamicAPIModuleManager();
            return vrDynamicAPIModuleManager.GetAllVRDynamicAPIModulesNames();
        }

    }

}
