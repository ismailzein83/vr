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
    public class DataSourceImportedBatchController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
        {
            DataSourceImportedBatchManager manager = new DataSourceImportedBatchManager();
            return GetWebResponse(input, manager.GetFilteredDataSourceImportedBatches(input));
        }

        [HttpGet]
        public List<Vanrise.Integration.Entities.DataSourceImportedBatchName> GetBatchNames()
        {
            DataSourceImportedBatchManager manager = new DataSourceImportedBatchManager();
            List<DataSourceImportedBatchName> response = manager.GetBatchNames();
            return response;
        }
    }
}