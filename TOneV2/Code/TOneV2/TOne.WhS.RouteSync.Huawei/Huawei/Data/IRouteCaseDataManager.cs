using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Huawei.Data
{
    public interface IRouteCaseDataManager : IDataManager, IBulkApplyDataManager<RouteCase>
    {
        string SwitchId { get; set; }
        List<RouteCase> GetAllRouteCases();
        List<RouteCase> GetNotSyncedRouteCases();
        void Initialize(IRouteCaseInitializeContext context);
        Dictionary<string, RouteCase> GetRouteCasesAfterRCNumber(int rcNumber);
        void ApplyRouteCaseForDB(object preparedRouteCase);
        void UpdateSyncedRouteCases(IEnumerable<int> rcNumbers);
    }
}