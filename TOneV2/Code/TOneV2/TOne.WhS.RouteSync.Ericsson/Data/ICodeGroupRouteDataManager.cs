using System.Collections.Generic;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
    public interface ICodeGroupRouteDataManager : IDataManager, IBulkApplyDataManager<CodeGroupRoute>
    {
        string SwitchId { get; set; }
        void Initialize(ICodeGroupRouteInitializeContext context);
        void InsertRoutes(IEnumerable<CodeGroupRoute> routes);
        Dictionary<string, List<CodeGroupRoute>> GetFilteredCodeGroupRouteByBO(IEnumerable<string> customerBOs);
    }
}