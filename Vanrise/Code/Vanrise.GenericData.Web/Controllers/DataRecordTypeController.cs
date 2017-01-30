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
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordType")]
    public class DataRecordTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetDataRecordTypeToEdit")]
        public DataRecordType GetDataRecordTypeToEdit(Guid dataRecordTypeId)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            return dataRecordTypeManager.GetDataRecordTypeToEdit(dataRecordTypeId);
        }

        [HttpGet]
        [Route("GetDataRecordType")]
        public DataRecordType GetDataRecordType(Guid dataRecordTypeId)
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

        [Route("UpdateDataRecordType")]
        public Vanrise.Entities.UpdateOperationOutput<DataRecordTypeDetail> UpdateDataRecordType(DataRecordType dataRecordType)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            return dataRecordTypeManager.UpdateDataRecordType(dataRecordType);
        }

        [HttpPost]
        [Route("AddDataRecordType")]
        public Vanrise.Entities.InsertOperationOutput<DataRecordTypeDetail> AddDataRecordType(DataRecordType dataRecordType)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            return dataRecordTypeManager.AddDataRecordType(dataRecordType);
        }
        [HttpGet]
        [Route("GetDataRecordTypeInfo")]
        public IEnumerable<DataRecordTypeInfo> GetDataRecordTypeInfo(string serializedFilter = null)
        {
            DataRecordTypeInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<DataRecordTypeInfoFilter>(serializedFilter) : null;
            DataRecordTypeManager manager = new DataRecordTypeManager();
            return manager.GetDataRecordTypeInfo(filter);
        }

        [HttpGet]
        [Route("GetDataRecordTypeExtraFieldsTemplates")]
        public IEnumerable<DataRecordTypeExtraFieldTemplate> GetDataRecordTypeExtraFieldsTemplates()
        {
            DataRecordTypeManager manager = new DataRecordTypeManager();
            return manager.GetDataRecordTypeExtraFieldsTemplates();
        }

    }
}