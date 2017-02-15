using System;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataStorages.DataRecordStorage;

namespace Vanrise.GenericData.MainExtensions.DataStorages.DataStore
{
    public class VRRestAPIRecordDataManager : IRemoteRecordDataManager
    {
        Guid _connectionId;
        Entities.DataRecordStorage _dataRecordStorage;

        public VRRestAPIRecordDataManager(Entities.DataRecordStorage dataRecordStorage, Guid connectionId)
        {
            _dataRecordStorage = dataRecordStorage;
            _connectionId = connectionId;
        }

        public IDataRetrievalResult<DataRecordDetail> GetFilteredDataRecords(DataRetrievalInput<DataRecordQuery> input)
        {
            if (_dataRecordStorage.Settings == null)
                throw new NullReferenceException(String.Format("dataRecordStorage.Settings Id '{0}'", _dataRecordStorage.DataRecordStorageId));

            var restAPIDataRecordStorageSettings = _dataRecordStorage.Settings as VRRestAPIDataRecordStorageSettings;
            if (restAPIDataRecordStorageSettings == null)
                throw new NullReferenceException(String.Format("dataRecordStorage.Settings is not of type VRRestAPIDataRecordStorageSettings for dataRecordStorage Id '{0}'", _dataRecordStorage.DataRecordStorageId));

            input.Query.DataRecordStorageIds = restAPIDataRecordStorageSettings.RemoteDataRecordStorageIds;

            VRRestAPIRecordQueryInterceptor vrRestAPIRecordQueryInterceptor = restAPIDataRecordStorageSettings.VRRestAPIRecordQueryInterceptor as VRRestAPIRecordQueryInterceptor;
            if (vrRestAPIRecordQueryInterceptor == null)
                throw new NullReferenceException("restAPIDataRecordStorageSettings.DataRecordQueryInterceptor should be of type VRRestAPIRecordQueryInterceptor");

            vrRestAPIRecordQueryInterceptor.PrepareQuery(new VRRestAPIRecordQueryInterceptorContext() { Query = input.Query });

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(_connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            return connectionSettings.Post<DataRetrievalInput<DataRecordQuery>, DataRecordDetailBigResult<DataRecordDetail>>("/api/VR_GenericData/DataRecordStorageLog/GetFilteredDataRecordStorageLogs", input, true);
        }
    }
}