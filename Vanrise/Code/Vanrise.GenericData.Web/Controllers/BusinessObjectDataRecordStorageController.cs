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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessObjectDataRecordStorage")]
    public class BusinessObjectDataRecordStorageController : BaseAPIController
    {

        [HttpGet]
        [Route("GetDataRecordStorageTemplateConfigs")]
        public IEnumerable<BusinessObjectDataRecordStorageConfig> GetDataRecordStorageTemplateConfigs()
        {
            BusinessObjectDataRecordStorageManager manager = new BusinessObjectDataRecordStorageManager();
            return manager.GetDataRecordStorageTemplateConfigs();
        }

    }
}