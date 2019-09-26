using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace NetworkProvision.Business
{
    //public class ActionHandlerApiDiscovery : VRAPIDiscovery
    //{
    //    public override List<Type> GetControllerTypes(IVRAPIDiscoveryGetControllerTypesContext context)
    //    {
    //        ActionHandlerManager actionHandlerManager = new ActionHandlerManager();
    //        return actionHandlerManager.BuildAllActionHandlerControllers();
    //    }

    //    public override bool IsCacheExpired(ref DateTime? lastCheckTime)
    //    {
    //        return Vanrise.Caching.CacheManagerFactory.GetCacheManager<ActionHandlerManager.CacheManager>().IsCacheExpired(ref lastCheckTime);
    //    }

    //    public override List<string> GetModuleNames(IVRAPIDiscoveryGetModuleNamesContext context)
    //    {
    //        return new List<string> { "NetworkProvision" };
    //    }
    //}
}
