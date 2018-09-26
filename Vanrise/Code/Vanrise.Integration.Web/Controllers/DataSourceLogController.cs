using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Integration.Business;
using Vanrise.Web.Base;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataSourceLog")]
    public class DataSourceLogController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredDataSourceLogs")]
        public object GetFilteredDataSourceLogs(Vanrise.Entities.DataRetrievalInput<DataSourceLogQuery> input)
        {
            DataSourceLogManager manager = new DataSourceLogManager();
            return GetWebResponse(input, manager.GetFilteredDataSourceLogs(input), "Data Source Logs");
        }
    }
}