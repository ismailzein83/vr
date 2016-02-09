(function (appControllers) {

    'use strict';

    DataStoreAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataStoreAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
       

        function GetDataStoresInfo(input) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataStore', 'GetDataStoresInfo'), input);
        }

        function GetFilteredDataStores(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataStore', 'GetFilteredDataStores'), input);
        }

        function GetDataStore(dataStoreId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataStore', 'GetDataStore'), {
                dataStoreId: dataStoreId
            });
        }

        function AddDataStore(dataStore) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataStore', 'AddDataStore'), dataStore)
        }

        function UpdateDataStore(dataStore) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataStore', 'UpdateDataStore'), dataStore);
        }



        return {
            GetDataStoresInfo: GetDataStoresInfo,
            GetFilteredDataStores :GetFilteredDataStores ,
            GetDataStore : GetDataStore ,
            AddDataStore : AddDataStore ,
            UpdateDataStore : UpdateDataStore
        };
    }

    appControllers.service('VR_GenericData_DataStoreAPIService', DataStoreAPIService);

})(appControllers);