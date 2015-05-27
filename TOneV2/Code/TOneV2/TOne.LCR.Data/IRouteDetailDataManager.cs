using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Data
{
    public interface IRouteDetailDataManager : IRoutingDataManager, IDataManager
    {
        void ApplyRouteDetailsToDB(Object preparedRouteDetails);

        void CreateIndexesOnTable();

        object InitialiazeStreamForDBApply();

        void WriteRouteToStream(RouteDetail routeDetail, object stream);

        object FinishDBApplyStream(object stream);

        List<RouteDetail> GetRoutesDetail(string customerId, string code, int? ourZoneId);

        IEnumerable<RouteDetail> GetRoutesDetail(IEnumerable<string> customerIds, string code, IEnumerable<int> zoneIds, int fromRow, int toRow, bool isDescending, string orderBy);
    }
}
