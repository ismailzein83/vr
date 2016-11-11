(function (appControllers) {

    'use strict';

    BusinessEntityDefinitionService.$inject = ['VRModalService', 'VRNotificationService'];

    function BusinessEntityDefinitionService(VRModalService, VRNotificationService) {
        return {
            editBusinessEntityDefinition: editBusinessEntityDefinition
        };

        function editBusinessEntityDefinition(businessEntityDefinitionId, onBusinessEntityDefinitionUpdated, editor) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBusinessEntityDefinitionUpdated = onBusinessEntityDefinitionUpdated;
            };

            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId
            };

            VRModalService.showModal(editor, parameters, modalSettings);
        }

    }

    appControllers.service('VR_GenericData_BusinessEntityDefinitionService', BusinessEntityDefinitionService);

})(appControllers);