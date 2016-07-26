
(function (appControllers) {

    "use strict";

    ReprocessDefinitionService.$inject = ['VRModalService'];

    function ReprocessDefinitionService(VRModalService) {

        function addReprocessDefinition(onReprocessDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onReprocessDefinitionAdded = onReprocessDefinitionAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/ReprocessDefinition/ReprocessDefinitionEditor.html', null, settings);
        };

        function editReprocessDefinition(styleDefinitionId, onReprocessDefinitionUpdated) {
            var settings = {};

            var parameters = {
                styleDefinitionId: styleDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onReprocessDefinitionUpdated = onReprocessDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/ReprocessDefinition/ReprocessDefinitionEditor.html', parameters, settings);
        }


        return {
            addReprocessDefinition: addReprocessDefinition,
            editReprocessDefinition: editReprocessDefinition
        };
    }

    appControllers.service('Reprocess_ReprocessDefinitionService', ReprocessDefinitionService);

})(appControllers);