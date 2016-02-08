(function (appControllers) {

    'use strict';

    DataRecordStorageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataRecordStorageAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetFilteredDataRecordStorages: GetFilteredDataRecordStorages
        };

        function GetFilteredDataRecordStorages(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataRecordStorage', 'GetFilteredDataRecordStorages'), input);
        }
    }

    appControllers.service('VR_GenericData_DataRecordStorageAPIService', DataRecordStorageAPIService);

})(appControllers);