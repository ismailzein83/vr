using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Integration.Business;
using Vanrise.Web.Base;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Integration.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataSourceImportedBatch")]
    public class DataSourceImportedBatchController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredDataSourceImportedBatches")]
        public object GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
        {
            DataSourceImportedBatchManager manager = new DataSourceImportedBatchManager();
            return GetWebResponse(input, manager.GetFilteredDataSourceImportedBatches(input), "Data Source Imported Batches");
        }

    }
}