using System;
using Vanrise.Data.SQL;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Data;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
    public class RouteDataManager : BaseSQLDataManager, IRouteDataManager
    {
        public RouteDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(ConvertedRoute record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            throw new NotImplementedException();
        }
    }
}