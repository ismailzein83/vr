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
            if (context.Switch != null && context.Switch.Settings != null && context.Switch.Settings.RouteSynchronizer != null)
                return context.Switch.Settings.RouteSynchronizer.SupportSyncWithinRouteBuild;

            return false;
        }
    }
}
