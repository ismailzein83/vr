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
        [Route("GetRemoteAnalyticTablesInfo")]
        public IEnumerable<AnalyticTableInfo> GetRemoteAnalyticTablesInfo(Guid connectionId, string filter = null)
        {
            AnalyticTableManager manager = new AnalyticTableManager();
            return manager.GetRemoteAnalyticTablesInfo(connectionId, filter);
        }
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
        [HttpGet]
        [Route("GetTableById")]
        public AnalyticTable GetTableById(Guid tableId)
        {
            AnalyticTableManager manager = new AnalyticTableManager();
            return manager.GetAnalyticTableById(tableId);
        }
        [HttpPost]
        [Route("UpdateAnalyticTable")]
        public Vanrise.Entities.UpdateOperationOutput<AnalyticTableDetail> UpdateAnalyticTable(AnalyticTable analyticTable)
        {
            AnalyticTableManager manager = new AnalyticTableManager();
            return manager.UpdateAnalyticTable(analyticTable);
        }

        [HttpPost]
        [Route("AddAnalyticTable")]
        public Vanrise.Entities.InsertOperationOutput<AnalyticTableDetail> AddAnalyticTable(AnalyticTable analyticTable)
        {
            AnalyticTableManager manager = new AnalyticTableManager();
            return manager.AddAnalyticTable(analyticTable);
        }
    }
}