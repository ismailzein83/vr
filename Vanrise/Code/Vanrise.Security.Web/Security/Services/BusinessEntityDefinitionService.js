(function (appControllers) {

    'use strict';

    BusinessEntityDefinitionService.$inject = ['UtilsService', 'VRModalService', 'VRNotificationService'];

    function BusinessEntityDefinitionService(UtilsService, VRModalService, VRNotificationService) {
        return ({
            addBusinessEntityDefinition: addBusinessEntityDefinition,
            updateBusinessEntityDefinition: updateBusinessEntityDefinition,
        });

        function addBusinessEntityDefinition(onBusinessEntityDefinitionAdded, moduleId) {
            var modalSettings = {
            };
            var parameters = {
                moduleId: moduleId
            }
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBusinessEntityDefinitionAdded = onBusinessEntityDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/BusinessEntity/BusinessEntityDefinitionEditor.html', parameters, modalSettings);
        }

        function updateBusinessEntityDefinition(entityId, onBusinessEntityDefinitionUpdated) {
            var modalParameters = {
                entityId: entityId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBusinessEntityDefinitionUpdated = onBusinessEntityDefinitionUpdated
            };

            VRModalService.showModal('/Client/Modules/Security/Views/BusinessEntity/BusinessEntityDefinitionEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_Sec_BusinessEntityDefinitionService', BusinessEntityDefinitionService);

})(appControllers);
