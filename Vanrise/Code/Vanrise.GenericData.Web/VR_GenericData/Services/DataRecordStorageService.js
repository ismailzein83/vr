(function (appControllers) {

    'use strict';

    DataRecordStorageService.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function DataRecordStorageService(VR_GenericData_GenericRuleDefinitionAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
      var drillDownDefinitions = [];
        return {
            addDataRecordStorage: addDataRecordStorage,
            editDataRecordStorage: editDataRecordStorage,
            registerObjectTrackingDrillDownToDataRecordStorage: registerObjectTrackingDrillDownToDataRecordStorage,
            getDrillDownDefinition: getDrillDownDefinition,
            addRDBJoinDataRecordStorage: addRDBJoinDataRecordStorage,
            editRDBJoinDataRecordStorage: editRDBJoinDataRecordStorage,
            addRDBExpressionField: addRDBExpressionField,
            editRDBExpressionField: editRDBExpressionField
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

        function addRDBJoinDataRecordStorage(context, onJoinAdded) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onJoinAdded = onJoinAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordStorage/DataRecordStorageRDBJoinsEditor.html', modalParameters, modalSettings);
        }

        function editRDBJoinDataRecordStorage(joinEntity, context, onJoinUpdated) {
            var modalParameters = {
                joinEntity: joinEntity,
                context: context
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onJoinUpdated = onJoinUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordStorage/DataRecordStorageRDBJoinsEditor.html', modalParameters, modalSettings);
        }

        function addRDBExpressionField(context, onExpressionFieldAdded) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExpressionFieldAdded = onExpressionFieldAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordStorage/DataRecordStorageRDBExpressionFieldsEditor.html', modalParameters, modalSettings);
        }

        function editRDBExpressionField(expressionFieldEntity, context, onExpressionFieldUpdated) {
            var modalParameters = {
                expressionFieldEntity: expressionFieldEntity,
                context: context
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExpressionFieldUpdated = onExpressionFieldUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordStorage/DataRecordStorageRDBExpressionFieldsEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_GenericData_DataRecordStorageService', DataRecordStorageService);

})(appControllers);