using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordStorage")]
    public class DataRecordStorageController : BaseAPIController
    {
        DataRecordStorageManager _manager = new DataRecordStorageManager();

        [HttpPost]
        [Route("GetFilteredDataRecordStorages")]
        public object GetFilteredDataRecordStorages(Vanrise.Entities.DataRetrievalInput<DataRecordStorageQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredDataRecordStorages(input));
        }
    }
}