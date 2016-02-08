(function (appControllers) {

    'use strict';

    DataStoreAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataStoreAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetDataStoresInfo: GetDataStoresInfo
        };

        function GetDataStoresInfo(input) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataStore', 'GetDataStoresInfo'), input);
        }
    }

    appControllers.service('VR_GenericData_DataStoreAPIService', DataStoreAPIService);

})(appControllers);