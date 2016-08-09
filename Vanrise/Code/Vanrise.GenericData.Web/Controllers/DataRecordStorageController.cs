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
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordStorage")]
    public class DataRecordStorageController : BaseAPIController
    {
        DataRecordStorageManager _manager = new DataRecordStorageManager();

        [HttpPost]
        [Route("GetFilteredDataRecordStorages")]
        public object GetFilteredDataRecordStorages(Vanrise.Entities.DataRetrievalInput<DataRecordStorageQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredDataRecordStorages(input));
        }

        [HttpGet]
        [Route("GetDataRecordsStorageInfo")]
        public IEnumerable<DataRecordStorageInfo> GetDataRecordsStorageInfo(string filter)
        {
            DataRecordStorageFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<DataRecordStorageFilter>(filter) : null;
            return _manager.GetDataRecordsStorageInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetDataRecordStorage")]
        public DataRecordStorage GetDataRecordStorage(int dataRecordStorageId)
        {
            return _manager.GetDataRecordStorage(dataRecordStorageId);
        }

        [HttpPost]
        [Route("AddDataRecordStorage")]
        public Vanrise.Entities.InsertOperationOutput<DataRecordStorageDetail> AddDataRecordStorage(DataRecordStorage dataRecordStorage)
        {
            return _manager.AddDataRecordStorage(dataRecordStorage);
        }

        [HttpPost]
        [Route("UpdateDataRecordStorage")]
        public Vanrise.Entities.UpdateOperationOutput<DataRecordStorageDetail> UpdateDataRecordStorage(DataRecordStorage dataRecordStorage)
        {
            return _manager.UpdateDataRecordStorage(dataRecordStorage);
        }

        [HttpGet]
        [Route("CheckRecordStoragesAccess")]
        public List<int> CheckRecordStoragesAccess(List<int> dataRecordStorages)
        {
            throw new NotImplementedException();
        }
    }
}