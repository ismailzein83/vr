
(function (appControllers) {

    "use strict";

    ReprocessDefinitionService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function ReprocessDefinitionService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
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
        function getEntityUniqueName() {
            return "Reprocess_ReprocessDefinition";
        }

        function registerObjectTrackingDrillDownToReprocessDefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, reprocessDefinitionItem) {
                reprocessDefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: reprocessDefinitionItem.Entity.ReprocessDefinitionId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return reprocessDefinitionItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addReprocessDefinition: addReprocessDefinition,
            editReprocessDefinition: editReprocessDefinition,
            registerObjectTrackingDrillDownToReprocessDefinition: registerObjectTrackingDrillDownToReprocessDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('Reprocess_ReprocessDefinitionService', ReprocessDefinitionService);

})(appControllers);