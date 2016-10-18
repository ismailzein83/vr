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
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordFieldTypeConfig")]
    public class DataRecordFieldTypeConfigController : BaseAPIController
    {
        //[HttpGet]
        //[Route("GetDataRecordFieldTypes")]
        //public IEnumerable<DataRecordFieldTypeConfig> GetDataRecordFieldTypes()
        //{
        //    DataRecordFieldTypeConfigManager manager = new DataRecordFieldTypeConfigManager();
        //    return manager.GetDataRecordFieldTypes();
        //}

        //[HttpGet]
        //[Route("GetDataRecordFieldTypeConfig")]
        //public DataRecordFieldTypeConfig GetDataRecordFieldTypeConfig(int configId)
        //{
        //    DataRecordFieldTypeConfigManager manager = new DataRecordFieldTypeConfigManager();
        //    return manager.GetDataRecordFieldTypeConfig(configId);
        //}

    }
}