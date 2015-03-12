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
        Object PrepareRouteDetailsForDBApply(List<RouteDetail> routeDetails);

        void ApplyRouteDetailsToDB(Object preparedRouteDetails);

        void CreateIndexesOnTable();

        object InitialiazeStreamForDBApply();

        void WriteRouteToStream(RouteDetail routeDetail, object stream);

        object FinishDBApplyStream(object stream);
    }
}
