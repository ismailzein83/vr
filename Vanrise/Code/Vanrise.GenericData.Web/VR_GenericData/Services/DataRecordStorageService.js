(function (appControllers) {

    'use strict';

    DataRecordStorageService.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function DataRecordStorageService(VR_GenericData_GenericRuleDefinitionAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
      var drillDownDefinitions = [];
        return {
            addDataRecordStorage: addDataRecordStorage,
            editDataRecordStorage: editDataRecordStorage,
            registerObjectTrackingDrillDownToDataRecordStorage: registerObjectTrackingDrillDownToDataRecordStorage,
            getDrillDownDefinition: getDrillDownDefinition
        };

        function addDataRecordStorage(onDataRecordStorageAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordStorageAdded = onDataRecordStorageAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordStorage/DataRecordStorageEditor.html', undefined, modalSettings);
        }

        function editDataRecordStorage(dataRecordStorageId, onDataRecordStorageUpdated) {
            var modalParameters = {
                DataRecordStorageId: dataRecordStorageId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordStorageUpdated = onDataRecordStorageUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordStorage/DataRecordStorageEditor.html', modalParameters, modalSettings);
        }

        function getEntityUniqueName() {
            return "VR_GenericData_DataRecordStorage";
        }

        function registerObjectTrackingDrillDownToDataRecordStorage() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, dataRecordStorageItem) {
                dataRecordStorageItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: dataRecordStorageItem.Entity.DataRecordStorageId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return dataRecordStorageItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    }

    appControllers.service('VR_GenericData_DataRecordStorageService', DataRecordStorageService);

})(appControllers);