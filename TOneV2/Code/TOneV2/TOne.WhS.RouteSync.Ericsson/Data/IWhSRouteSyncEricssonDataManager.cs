using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Entities;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
    public interface IWhSRouteSyncEricssonDataManager : IDataManager
    {
        string SwitchId { get; set; }
        void Initialize(IWhSRouteSyncEricssonInitializeContext context);
    }
}
