
(function (appControllers) {

    'use stict';

    StatusDefinitionService.$inject = ['VRModalService'];

    function StatusDefinitionService(VRModalService) {

        function addStatusDefinition(onStatusDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onStatusDefinitionAdded = onStatusDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Common/Views/StatusDefinition/StatusDefinitionEditor.html', null, settings);
        };

        function editStatusDefinition(statusDefinitionId, onStatusDefinitionUpdated) {
            var settings = {};

            var parameters = {
                statusDefinitionId: statusDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onStatusDefinitionUpdated = onStatusDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/StatusDefinition/StatusDefinitionEditor.html', parameters, settings);
        }


        return {
            addStatusDefinition: addStatusDefinition,
            editStatusDefinition: editStatusDefinition
        };
    }

    appControllers.service('VR_Common_StatusDefinitionService', StatusDefinitionService);

})(appControllers);