using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteSyncSwitchGetter : SwitchInfoGetter
    {
        public override SwitchInfo GetSwitchInfo(ISwitchInfoGetterContext context)
        {
            int switchId = GetSwitchId(context);
            var sw = new SwitchManager().GetSwitch(switchId);
            return sw != null ? MapToSwitchInfo(sw) : null;
        }

        public override List<SwitchInfo> GetAllSwitchInfo(ISwitchInfoGetterAllContext context)
        {
            var allSwitches = new SwitchManager().GetAllSwitches();
            return allSwitches != null ? allSwitches.Select(sw => MapToSwitchInfo(sw)).ToList() : null;
        }

        private int GetSwitchId(ISwitchInfoGetterContext context)
        {
            int switchId;
            if (!int.TryParse(context.SwitchId, out switchId))
                throw new Exception(String.Format("Invalid Switch Id '{0}'", context.SwitchId));
            return switchId;
        }

        private SwitchInfo MapToSwitchInfo(Entities.Switch sw)
        {
            return new SwitchInfo
            {
                SwitchId = sw.SwitchId.ToString(),
                Name = sw.Name,
                RouteSynchronizer = sw.Settings != null ? sw.Settings.RouteSynchronizer : null
            };
        }
    }
}
