(function (appControllers) {

    'use strict';

    DataTransformationDefinitionService.$inject = ['VRModalService'];

    function DataTransformationDefinitionService(VRModalService) {
        return ({
            addDataTransformationDefinition: addDataTransformationDefinition,
            editDataTransformationDefinition: editDataTransformationDefinition,
            addDataRecordType: addDataRecordType,
            editDataRecordType: editDataRecordType,
            deleteDataRecordType: deleteDataRecordType

        });

        function addDataTransformationDefinition(onDataTransformationDefinitionAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataTransformationDefinitionAdded = onDataTransformationDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataTransformationDefinition/DataTransformationDefinitionEditor.html', null, modalSettings);
        }

        function editDataTransformationDefinition(dataRecordTypeId, onDataTransformationDefinitionUpdated) {
            var modalParameters = {
                DataTransformationDefinitionId: dataRecordTypeId
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
    };

    appControllers.service('VR_GenericData_DataTransformationDefinitionService', DataTransformationDefinitionService);

})(appControllers);
