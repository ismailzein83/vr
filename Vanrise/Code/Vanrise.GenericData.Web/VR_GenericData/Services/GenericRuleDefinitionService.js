(function (appControllers) {

    'use strict';

    GenericRuleDefinitionService.$inject = ['VRModalService'];

    function GenericRuleDefinitionService(VRModalService) {
        return {
            editGenericRuleDefinition: editGenericRuleDefinition
        };

        function editGenericRuleDefinition(genericRuleDefinitionId, onGenericRuleDefinitionUpdated) {
            var modalParameters = {
                genericRuleDefinitionId: genericRuleDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleDefinitionUpdated = onGenericRuleDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_GenericData_GenericRuleDefinitionService', GenericRuleDefinitionService);

})(appControllers);