(function (appControllers) {

    'use strict';

    DataRecordStorageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataRecordStorageAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetFilteredDataRecordStorages: GetFilteredDataRecordStorages,
            GetDataRecordStorage: GetDataRecordStorage,
            AddDataRecordStorage: AddDataRecordStorage,
            UpdateDataRecordStorage: UpdateDataRecordStorage
        };

        function GetFilteredDataRecordStorages(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'GetFilteredDataRecordStorages'), input);
        }

        function GetDataRecordStorage(dataRecordStorageId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'GetDataRecordStorage'), {
                dataRecordStorageId: dataRecordStorageId
            });
        }

        function AddDataRecordStorage(dataRecordStorage) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'AddDataRecordStorage'), dataRecordStorage);
        }

        function UpdateDataRecordStorage(dataRecordStorage) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'UpdateDataRecordStorage'), dataRecordStorage);
        }
    }

    appControllers.service('VR_GenericData_DataRecordStorageAPIService', DataRecordStorageAPIService);

})(appControllers);