
(function (appControllers) {

    "use strict";

    ReprocessFilterFieldDefinitionService.$inject = ['VRModalService'];

    function ReprocessFilterFieldDefinitionService(VRModalService) {

        function addReprocessFilterFieldDefinition(onReprocessFilterFieldDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onReprocessFilterFieldDefinitionAdded = onReprocessFilterFieldDefinitionAdded;
            };
            VRModalService.showModal('/Client/Modules/Reprocess/Views/ReprocessFilterFieldDefinition/ReprocessFilterFieldDefinitionEditor.html', null, settings);
        };

        function editReprocessFilterFieldDefinition(reprocessFilterFieldDefinition, onReprocessFilterFieldDefinitionUpdated) {
            var settings = {};

            var parameters = {
                reprocessFilterFieldDefinition: reprocessFilterFieldDefinition,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onReprocessFilterFieldDefinitionUpdated = onReprocessFilterFieldDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Reprocess/Views/ReprocessFilterFieldDefinition/ReprocessFilterFieldDefinitionEditor.html', parameters, settings);
        }

        return {
            addReprocessFilterFieldDefinition: addReprocessFilterFieldDefinition,
            editReprocessFilterFieldDefinition: editReprocessFilterFieldDefinition
        };
    }

    appControllers.service('Reprocess_ReprocessFilterFieldDefinitionService', ReprocessFilterFieldDefinitionService);

})(appControllers);