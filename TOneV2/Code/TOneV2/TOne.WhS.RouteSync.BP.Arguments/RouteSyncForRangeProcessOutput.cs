using System;
using System.Collections.Concurrent;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.BP.Arguments
{
    public class RouteSyncForRangeProcessOutput
    {
        public ConcurrentDictionary<string, SwitchSyncOutput> SwitchSyncOutputDict { get; set; }
    }
}
