(function (appControllers) {

    'use strict';

    BusinessEntityDefinitionService.$inject = ['VRModalService', 'VRNotificationService'];

    function BusinessEntityDefinitionService(VRModalService, VRNotificationService) {
        return {
            editBusinessEntityDefinition: editBusinessEntityDefinition,
            addBusinessEntityDefinition: addBusinessEntityDefinition,
        };

        function editBusinessEntityDefinition(businessEntityDefinitionId, onBusinessEntityDefinitionUpdated) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBusinessEntityDefinitionUpdated = onBusinessEntityDefinitionUpdated;
            };

            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId,
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Definition/GenericBEEditorDefintion.html', parameters, modalSettings);
        }
        function addBusinessEntityDefinition(onBusinessEntityDefinitionAdded) {
            var modalParameters;

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBusinessEntityDefinitionAdded = onBusinessEntityDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Definition/GenericBEEditorDefintion.html', modalParameters, modalSettings);
        }

    }

    appControllers.service('VR_GenericData_BusinessEntityDefinitionService', BusinessEntityDefinitionService);

})(appControllers);