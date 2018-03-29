using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
    public interface IRouteDataManager : IBulkApplyDataManager<ConvertedRoute>, IDataManager
    {
        void Initialize(ISwitchRouteSynchronizerInitializeContext context);
    }
}