
(function (appControllers) {

    'use stict';

    StatusDefinitionService.$inject = ['VRModalService'];

    function StatusDefinitionService(VRModalService) {

        function addStatusDefinition(onStatusDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onStatusDefinitionAdded = onStatusDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Status/StatusDefinitionEditor.html', null, settings);
        };

        function editStatusDefinition(statusDefinitionId, onStatusDefinitionUpdated) {
            var settings = {};

            var parameters = {
                statusDefinitionId: statusDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onStatusDefinitionUpdated = onStatusDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Status/StatusDefinitionEditor.html', parameters, settings);
        }


        return {
            addStatusDefinition: addStatusDefinition,
            editStatusDefinition: editStatusDefinition
        };
    }

    appControllers.service('Retail_BE_StatusDefinitionService', StatusDefinitionService);

})(appControllers);