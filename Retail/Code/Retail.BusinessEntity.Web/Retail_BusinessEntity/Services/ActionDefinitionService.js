(function (appControllers) {

    'use stict';

    ActionDefinitionService.$inject = ['VRModalService'];

    function ActionDefinitionService(VRModalService) {

        function addActionDefinition(onActionDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onActionDefinitionAdded = onActionDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Action/Definition/ActionDefinitionEditor.html', null, settings);
        };

        function editActionDefinition(actionDefinitionId, onActionDefinitionUpdated) {
            var modalSettings = {
            };

            var parameters = {
                actionDefinitionId: actionDefinitionId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onActionDefinitionUpdated = onActionDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Action/Definition/ActionDefinitionEditor.html', parameters, modalSettings);
        }

        return {
            addActionDefinition: addActionDefinition,
            editActionDefinition: editActionDefinition
        };
    }

    appControllers.service('Retail_BE_ActionDefinitionService', ActionDefinitionService);

})(appControllers);