using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class BuiltInIVSwitchSWSync : IVSwitchSWSync
    {
        public override Guid ConfigId { get { return new Guid("1EE51230-FE31-4D01-9289-0E27E24D3601"); } }
        public override PreparedConfiguration GetPreparedConfiguration()
        {
            return PreparedConfiguration.GetBuiltInCachedPreparedConfiguration(this);
        }
    }
}
