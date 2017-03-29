(function (appControllers) {

    'use strict';

    DataStoreService.$inject = ['VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function DataStoreService(VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
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

        function getEntityUniqueName() {
            return "VR_GenericData_DataStore";
        }

        function registerObjectTrackingDrillDownToDataStore() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, dataStoreItem) {
                dataStoreItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: dataStoreItem.Entity.DataStoreId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return dataStoreItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return ({
            editDataStore: editDataStore,
            addDataStore: addDataStore,
            registerObjectTrackingDrillDownToDataStore: registerObjectTrackingDrillDownToDataStore,
            getDrillDownDefinition: getDrillDownDefinition
        })
    }

    appControllers.service('VR_GenericData_DataStoreService', DataStoreService);

})(appControllers);
