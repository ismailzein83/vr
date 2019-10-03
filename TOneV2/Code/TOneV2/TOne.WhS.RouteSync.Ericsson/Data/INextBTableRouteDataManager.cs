using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
    public interface INextBTableRouteDataManager : IDataManager, IBulkApplyDataManager<NextBTableConvertedRoute>
    {
        string SwitchId { get; set; }

        List<NextBTableConvertedRoute> GetAllNextBTableRoute();

        void Initialize(INextBTableRouteInitializeContext context);

        void InsertNextBTableRoutes(IEnumerable<NextBTableConvertedRoute> routes);
    }
}
