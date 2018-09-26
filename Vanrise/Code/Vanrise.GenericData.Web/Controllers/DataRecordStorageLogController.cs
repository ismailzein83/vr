using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Entities.DataStorage.DataRecordStorage;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordStorageLog")]
    public class DataRecordStorageLogController : BaseAPIController
    {
        DataRecordStorageManager _manager = new DataRecordStorageManager();

        [HttpPost]
        [Route("GetFilteredDataRecordStorageLogs")]
        public object GetFilteredDataRecordStorageLogs(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input)
        {
            if (!_manager.DoesUserHaveAccess(input))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredDataRecords(input), input.Query.ReportName);
        }
    }
}