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
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataStore")]
    public class DataStoreController : BaseAPIController
    {
        DataStoreManager _manager = new DataStoreManager();

        [HttpGet]
        [Route("GetDataStoresInfo")]
        public IEnumerable<DataStoreInfo> GetDataStoresInfo()
        {
            return _manager.GetDataStoresInfo();
        }
    }
}