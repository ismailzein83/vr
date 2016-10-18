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
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataStoreConfig")]
    public class DataStoreConfigController:BaseAPIController
    {
        //[HttpGet]
        //[Route("GetDataStoreConfigs")]
        //public IEnumerable<DataStoreConfig> GetDataStoreConfigs()
        //{
        //    DataStoreConfigManager manager = new DataStoreConfigManager();
        //    return manager.GetDataStoreConfigs();
        //}

        //[HttpGet]
        //[Route("GetDataStoreConfig")]
        //public DataStoreConfig GetDataStoreConfig(int dataStoreConfigId)
        //{
        //    DataStoreConfigManager manager = new DataStoreConfigManager();
        //    return manager.GeDataStoreConfigById(dataStoreConfigId);
        //}
    }
}