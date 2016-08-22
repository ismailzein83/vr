using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Business
{
    public class SwitchManager
    {
        public List<SwitchInfo> GetSwitches(IEnumerable<string> switchIds)
        {
            var switchInfoGetter = new ConfigManager().GetSwitchInfoGetter();
            if (switchInfoGetter == null)
                throw new NullReferenceException("switchInfoGetter");
            return switchIds.Select(switchId => switchInfoGetter.GetSwitchInfo(new SwitchInfoGetterContext { SwitchId = switchId })).ToList();
        }

        public List<SwitchInfo> GetAllSwitches()
        {
            var switchInfoGetter = new ConfigManager().GetSwitchInfoGetter();
            if (switchInfoGetter == null)
                throw new NullReferenceException("switchInfoGetter");
            return switchInfoGetter.GetAllSwitchInfo(new SwitchInfoGetterAllContext());
        }
    }
}
