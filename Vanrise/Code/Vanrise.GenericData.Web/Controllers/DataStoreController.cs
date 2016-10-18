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
        [Route("GetDataStoreConfigs")]
        public IEnumerable<DataStoreConfig> GetDataStoreConfigs()
        {
            return _manager.GetDataStoreConfigs();
        }

        [HttpPost]
        [Route("GetFilteredDataStores")]
        public object GetFilteredDataStores(Vanrise.Entities.DataRetrievalInput<DataStoreQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredDataStores(input));
        }

        [HttpGet]
        [Route("GetDataStoresInfo")]
        public IEnumerable<DataStoreInfo> GetDataStoresInfo()
        {
            return _manager.GetDataStoresInfo();
        }

        [HttpGet]
        [Route("GetDataStore")]
        public DataStore GetDataStore(Guid dataStoreId)
        {
            return _manager.GeDataStore(dataStoreId);
        }

        [HttpPost]
        [Route("AddDataStore")]
        public Vanrise.Entities.InsertOperationOutput<DataStoreDetail> AddDataStore(DataStore dataStore)
        {
            return _manager.AddDataStore(dataStore);
        }

        [HttpPost]
        [Route("UpdateDataStore")]
        public Vanrise.Entities.UpdateOperationOutput<DataStoreDetail> UpdateDataStore(DataStore dataStore)
        {
            return _manager.UpdateDataStore(dataStore);
        }

    }
}