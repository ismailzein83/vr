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
    public class DataSourceImportedBatchController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
        {
            DataSourceImportedBatchManager manager = new DataSourceImportedBatchManager();
            return GetWebResponse(input, manager.GetFilteredDataSourceImportedBatches(input));
        }

        [HttpPost]
        public object GetQueueItemHeaders(Vanrise.Entities.DataRetrievalInput<List<long>> input)
        {
            DataSourceImportedBatchManager manager = new DataSourceImportedBatchManager();
            return GetWebResponse(input, manager.GetQueueItemHeaders(input));
        }
    }
}