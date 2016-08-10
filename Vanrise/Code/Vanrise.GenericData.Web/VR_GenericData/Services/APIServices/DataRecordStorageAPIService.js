(function (appControllers) {

    'use strict';

    DataRecordStorageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function DataRecordStorageAPIService(BaseAPIService, UtilsService, SecurityService , VR_GenericData_ModuleConfig) {
        return {
            GetFilteredDataRecordStorages: GetFilteredDataRecordStorages,
            GetDataRecordsStorageInfo: GetDataRecordsStorageInfo,
            GetDataRecordStorage: GetDataRecordStorage,
            AddDataRecordStorage: AddDataRecordStorage,
            HasAddDataRecordStorage :HasAddDataRecordStorage,
            UpdateDataRecordStorage: UpdateDataRecordStorage,
            HasUpdateDataRecordStorage: HasUpdateDataRecordStorage,
            CheckRecordStoragesAccess: CheckRecordStoragesAccess
        };

        function GetFilteredDataRecordStorages(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'GetFilteredDataRecordStorages'), input);
        }

        function GetDataRecordsStorageInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'GetDataRecordsStorageInfo'), {
                filter: filter
            });
        }
        function GetDataRecordStorage(dataRecordStorageId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'GetDataRecordStorage'), {
                dataRecordStorageId: dataRecordStorageId
            });
        }
        function AddDataRecordStorage(dataRecordStorage) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'AddDataRecordStorage'), dataRecordStorage);
        }
        function HasAddDataRecordStorage() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "DataRecordStorage", ['AddDataRecordStorage']));
        }
        function UpdateDataRecordStorage(dataRecordStorage) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'UpdateDataRecordStorage'), dataRecordStorage);
        }
        function HasUpdateDataRecordStorage() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "DataRecordStorage", ['UpdateDataRecordStorage']));
        }

        function CheckRecordStoragesAccess(dataRecordStorages) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'CheckRecordStoragesAccess'), dataRecordStorages);
        }
    }

    appControllers.service('VR_GenericData_DataRecordStorageAPIService', DataRecordStorageAPIService);

})(appControllers);