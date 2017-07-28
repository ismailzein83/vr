using System;
using System.Collections.Concurrent;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class BuildRoutesByCodePrefixOutput
    {
        public ConcurrentDictionary<string, SwitchSyncOutput> SwitchSyncOutputDict { get; set; }
    }
}