(function (appControllers) {

    'use strict';

    DataStoreAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function DataStoreAPIService(BaseAPIService, UtilsService, SecurityService , VR_GenericData_ModuleConfig) {
       

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
        function HasAddDataStore() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "DataStore", ['AddDataStore']));
        }
        function UpdateDataStore(dataStore) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataStore', 'UpdateDataStore'), dataStore);
        }
        function HasUpdateDataStore() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "DataStore", ['UpdateDataStore']));
        }
        function GetDataStoreConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataStore", "GetDataStoreConfigs"));
        }

        return {
            GetDataStoresInfo: GetDataStoresInfo,
            GetFilteredDataStores :GetFilteredDataStores ,
            GetDataStore : GetDataStore ,
            AddDataStore: AddDataStore,
            HasAddDataStore:HasAddDataStore,
            UpdateDataStore: UpdateDataStore,
            HasUpdateDataStore: HasUpdateDataStore,
            GetDataStoreConfigs: GetDataStoreConfigs
        };
    }

    appControllers.service('VR_GenericData_DataStoreAPIService', DataStoreAPIService);

})(appControllers);