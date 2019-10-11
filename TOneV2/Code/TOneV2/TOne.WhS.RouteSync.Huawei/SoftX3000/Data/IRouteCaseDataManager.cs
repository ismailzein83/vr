using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Data
{
    public interface IRouteCaseDataManager : IDataManager, IBulkApplyDataManager<RouteCase>
    {
        string SwitchId { get; set; }
        List<RouteCase> GetAllRouteCases();
        List<RouteCase> GetNotSyncedRouteCases();
        void Initialize(IRouteCaseInitializeContext context);
        List<RouteCase> GetRouteCasesAfterRCNumber(long rcNumber);
        void ApplyRouteCaseForDB(object preparedRouteCase);
        void UpdateSyncedRouteCases(IEnumerable<long> rcNumbers);
    }
}