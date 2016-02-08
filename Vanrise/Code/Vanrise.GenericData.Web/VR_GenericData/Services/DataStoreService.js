(function (appControllers) {

    'use strict';

    DataStoreService.$inject = ['VRModalService', 'VRNotificationService'];

    function DataStoreService(VRModalService, VRNotificationService) {
        function editDataStore(dataStoreId, onDataStoreUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataStoreUpdated = onDataStoreUpdated;
            };
            var parameters = {
                DataStoreId: dataStoreId
            };

            VRModalService.showModal("/Client/Modules/VR_GenericData/Views/DataStore/DataStoreEditor.html", parameters, settings);
        }

        function addDataStore(onDataStoreAdded) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDataStoreAdded = onDataStoreAdded;
            };
            var parameters = {};

            VRModalService.showModal("/Client/Modules/VR_GenericData/Views/DataStore/DataStoreEditor.html", parameters, settings);
        }

        return ({
            editDataStore: editDataStore,
            addDataStore: addDataStore
        })
    }

    appControllers.service('VR_GenericData_DataStoreService', DataStoreService);

})(appControllers);
