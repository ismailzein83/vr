using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Business
{
    public class SwitchInfoGetterContext : ISwitchInfoGetterContext
    {
        public string SwitchId
        {
            get;
            set;
        }
    }

    public class SwitchInfoGetterAllContext : ISwitchInfoGetterAllContext
    {
    }
}
