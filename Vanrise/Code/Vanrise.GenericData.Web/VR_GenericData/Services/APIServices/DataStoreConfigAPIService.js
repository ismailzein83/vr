(function (appControllers) {

    'use strict';

    DataStoreConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataStoreConfigAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
       
        function GetDataStoreConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataStoreConfig", "GetDataStoreConfigs"));
        }
        return ({
            GetDataStoreConfigs: GetDataStoreConfigs
        });
    }

    appControllers.service('VR_GenericData_DataStoreConfigAPIService', DataStoreConfigAPIService);

})(appControllers);
