
(function (appControllers) {

    "use strict";

    ReprocessDefinitionService.$inject = ['VRModalService'];

    function ReprocessDefinitionService(VRModalService) {

        function addReprocessDefinition(onReprocessDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onReprocessDefinitionAdded = onReprocessDefinitionAdded
            };
            VRModalService.showModal('/Client/Modules/Reprocess/Views/ReprocessDefinition/ReprocessDefinitionEditor.html', null, settings);
        };

        function editReprocessDefinition(reprocessDefinitionId, onReprocessDefinitionUpdated) {
            var settings = {};

            var parameters = {
                reprocessDefinitionId: reprocessDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onReprocessDefinitionUpdated = onReprocessDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Reprocess/Views/ReprocessDefinition/ReprocessDefinitionEditor.html', parameters, settings);
        }


        return {
            addReprocessDefinition: addReprocessDefinition,
            editReprocessDefinition: editReprocessDefinition
        };
    }

    appControllers.service('Reprocess_ReprocessDefinitionService', ReprocessDefinitionService);

})(appControllers);