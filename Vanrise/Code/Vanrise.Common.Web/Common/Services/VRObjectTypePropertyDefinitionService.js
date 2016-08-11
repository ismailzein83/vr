(function (appControllers) {

    'use strict';

    ObjectTypePropertyDefinitionService.$inject = ['VRModalService'];

    function ObjectTypePropertyDefinitionService(VRModalService) {

        function addObjectTypePropertyDefinition(properties, objectType, onObjectTypePropertyDefinitionAdded) {
            var modalParameters = {
                properties: properties,
                objectType: objectType
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onObjectTypePropertyDefinitionAdded = onObjectTypePropertyDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRObjectTypePropertyDefinition/VRObjectTypePropertyDefinitionEditor.html', modalParameters, modalSettings);
        }

        function editObjectTypePropertyDefinition(propertyName, properties, objectType, onObjectTypePropertyDefinitionUpdated) {
            var modalParameters = {
                propertyName: propertyName,
                properties: properties,
                objectType: objectType
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onObjectTypePropertyDefinitionUpdated = onObjectTypePropertyDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRObjectTypePropertyDefinition/VRObjectTypePropertyDefinitionEditor.html', modalParameters, modalSettings);
        }


        return {
            addObjectTypePropertyDefinition: addObjectTypePropertyDefinition,
            editObjectTypePropertyDefinition: editObjectTypePropertyDefinition
        };
    }

    appControllers.service('VRCommon_VRObjectTypePropertyDefinitionService', ObjectTypePropertyDefinitionService);

})(appControllers);