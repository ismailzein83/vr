using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordFields")]
    public class DataRecordFieldsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetDataRecordFieldTypeConfigs")]
        public IEnumerable<DataRecordFieldTypeConfig> GetDataRecordFieldTypeConfigs()
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetDataRecordFieldTypeConfigs();
        }

        [HttpGet]
        [Route("GetDataRecordFieldsInfo")]
        public IEnumerable<DataRecordFieldInfo> GetDataRecordFieldsInfo(string serializedFilter)
        {
            DataRecordFieldInfoFilter filter = !string.IsNullOrEmpty(serializedFilter) ? Vanrise.Common.Serializer.Deserialize<DataRecordFieldInfoFilter>(serializedFilter) : null;
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetDataRecordFieldsInfo(filter);
        }

        [HttpGet]
        [Route("GetDataRecordAttributes")]
        public List<DataRecordGridColumnAttribute> GetDataRecordAttributes(Guid dataRecordTypeId)
        {
            DataRecordTypeManager manager = new DataRecordTypeManager();
            return manager.GetDataRecordAttributes(dataRecordTypeId);
        }
        [HttpGet]
        [Route("GetDataRecordFieldFormulaExtensionConfigs")]
        public IEnumerable<DataRecordFieldFormulaConfig> GetDataRecordFieldFormulaExtensionConfigs()
        {
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetDataRecordFieldFormulaExtensionConfigs();
        }
    }
}