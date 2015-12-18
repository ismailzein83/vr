using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business.GenericDataRecord;
using Vanrise.Entities.GenericDataRecord;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordType")]
    [JSONWithTypeAttribute]
    public class DataRecordTypeController:BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredDataRecordTypes")]
        public object GetFilteredDataRecordTypes(Vanrise.Entities.DataRetrievalInput<DataRecordTypeQuery> input)
        {
            DataRecordTypeManager manager = new DataRecordTypeManager();
            return GetWebResponse(input, manager.GetFilteredDataRecordTypes(input));
        }

        [HttpGet]
        [Route("GetDataRecordType")]
        public DataRecordType GetDataRecordType(int dataRecordTypeId)
        {
            DataRecordTypeManager manager = new DataRecordTypeManager();
            return manager.GetDataRecordType(dataRecordTypeId);
        }
    }
}