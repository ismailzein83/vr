using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.IVSwitch;

namespace NP.IVSwitch.Data
{
    public interface IDataManager
    {
        BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
    }
}
