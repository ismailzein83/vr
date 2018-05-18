using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SyncWithinRouteBuildSwitchFilter : ISwitchFilter
    {
        public bool IsMatched(ISwitchFilterContext context)
        {
            context.Switch.ThrowIfNull("context.Switch");
            context.Switch.Settings.ThrowIfNull("context.Switch.Settings");
            context.Switch.Settings.RouteSynchronizer.ThrowIfNull("context.Switch.Settings.RouteSynchronizer");
            return context.Switch.Settings.RouteSynchronizer.SupportSyncWithinRouteBuild;
        }
    }
}
