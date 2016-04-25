using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AnalyticTable")]
    public class AnalyticTableController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetAnalyticTablesInfo")]
        public IEnumerable<AnalyticTableInfo> GetAnalyticTablesInfo(string filter = null)
        {
            AnalyticTableManager manager = new AnalyticTableManager();
            AnalyticTableInfoFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<AnalyticTableInfoFilter>(filter) : null;

            return manager.GetAnalyticTablesInfo(serializedFilter);
        }
        [HttpPost]
        [Route("GetFilteredAnalyticTables")]
        public object GetFilteredAnalyticTables(Vanrise.Entities.DataRetrievalInput<AnalyticTableQuery> input)
        {
            AnalyticTableManager manager = new AnalyticTableManager();
            return GetWebResponse(input, manager.GetFilteredAnalyticTables(input));
        }
    }
}