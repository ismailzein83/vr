(function (appControllers) {

    'use strict';

    DataTransformationDefinitionService.$inject = ['VRModalService'];

    function DataTransformationDefinitionService(VRModalService) {
        return ({
            addDataTransformationDefinition: addDataTransformationDefinition,
            editDataTransformationDefinition: editDataTransformationDefinition,
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
    };

    appControllers.service('VR_GenericData_DataTransformationDefinitionService', DataTransformationDefinitionService);

})(appControllers);
