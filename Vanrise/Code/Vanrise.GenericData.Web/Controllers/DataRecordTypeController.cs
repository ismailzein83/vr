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
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordType")]
    public class DataRecordTypeController:BaseAPIController
    {
        [HttpGet]
        [Route("GetDataRecordType")]
        public DataRecordType GetDataRecordType(int dataRecordTypeId)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            return dataRecordTypeManager.GetDataRecordType(dataRecordTypeId);
        }

        [HttpPost]
        [Route("GetFilteredDataRecordTypes")]
        public object GetFilteredDataRecordTypes(Vanrise.Entities.DataRetrievalInput<DataRecordTypeQuery> input)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            return GetWebResponse(input, dataRecordTypeManager.GetFilteredDataRecordTypes(input));
        }
    }
}