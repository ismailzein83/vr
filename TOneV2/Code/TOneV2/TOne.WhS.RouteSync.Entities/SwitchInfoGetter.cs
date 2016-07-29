using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public abstract class SwitchInfoGetter
    {
        public abstract SwitchInfo GetSwitchInfo(ISwitchInfoGetterContext context);
    }

    public interface ISwitchInfoGetterContext
    {
        string SwitchId { get; }
    }
}
