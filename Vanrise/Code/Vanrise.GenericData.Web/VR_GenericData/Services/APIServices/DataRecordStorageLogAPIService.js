(function (appControllers) {

    'use strict';

    DataRecordStorageLogAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function DataRecordStorageLogAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {
        return {
            GetFilteredDataRecordStorageLogs: GetFilteredDataRecordStorageLogs,
        };

        function GetFilteredDataRecordStorageLogs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorageLog', 'GetFilteredDataRecordStorageLogs'), input);
        }
    }

    appControllers.service('VR_GenericData_DataRecordStorageLogAPIService', DataRecordStorageLogAPIService);

})(appControllers);