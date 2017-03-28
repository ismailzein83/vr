(function (appControllers) {

    'use strict';

    DataTransformationDefinitionService.$inject = ['VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function DataTransformationDefinitionService(VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addDataTransformationDefinition: addDataTransformationDefinition,
            editDataTransformationDefinition: editDataTransformationDefinition,
            addDataRecordType: addDataRecordType,
            editDataRecordType: editDataRecordType,
            deleteDataRecordType: deleteDataRecordType,
            tryCompilationResult: tryCompilationResult,
            registerObjectTrackingDrillDownToDataTransformationdefinition: registerObjectTrackingDrillDownToDataTransformationdefinition,
            getDrillDownDefinition: getDrillDownDefinition

        });

        function addDataTransformationDefinition(onDataTransformationDefinitionAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataTransformationDefinitionAdded = onDataTransformationDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataTransformationDefinition/DataTransformationDefinitionEditor.html', null, modalSettings);
        }

        function editDataTransformationDefinition(dataTransformationDefinitionId, onDataTransformationDefinitionUpdated) {
            var modalParameters = {
                DataTransformationDefinitionId: dataTransformationDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataTransformationDefinitionUpdated = onDataTransformationDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataTransformationDefinition/DataTransformationDefinitionEditor.html', modalParameters, modalSettings);
        }

        function addDataRecordType(onDataRecordTypeAdded, existingTypes) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordTypeAdded = onDataRecordTypeAdded;
            };
            var modalParameters = {
                ExistingTypes: existingTypes
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataTransformationDefinition/TransformationRecordTypeEditor.html', modalParameters, modalSettings);
        }

        function editDataRecordType(dataRecordType, onDataRecordTypeUpdated, existingTypes) {
            var modalParameters = {
                DataRecordType: dataRecordType,
                ExistingTypes: existingTypes
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordTypeUpdated = onDataRecordTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataTransformationDefinition/TransformationRecordTypeEditor.html', modalParameters, modalSettings);
        }

        function deleteDataRecordType($scope, dataRecordTypeObj, onDataRecordTypeDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onDataRecordTypeDeleted(dataRecordTypeObj);
                    }
                });
        }

        function tryCompilationResult(errorMessages, dataTransformationObj) {
            var modalSettings = {};
            var modalParameters = {
                errorMessages: errorMessages,
                dataTransformationObj: dataTransformationObj
            };
            modalSettings.onScopeReady = function (modalScope) {
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataTransformationDefinition/DataTransformationCompilationResult.html', modalParameters, modalSettings);
        }
        function getEntityUniqueName() {
            return "VR_GenericData_DataTransformationDefinition";
        }

        function registerObjectTrackingDrillDownToDataTransformationdefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, dataTransformationdefinitionItem) {
                dataTransformationdefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: dataTransformationdefinitionItem.Entity.DataTransformationDefinitionId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return dataTransformationdefinitionItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    };

    appControllers.service('VR_GenericData_DataTransformationDefinitionService', DataTransformationDefinitionService);

})(appControllers);
