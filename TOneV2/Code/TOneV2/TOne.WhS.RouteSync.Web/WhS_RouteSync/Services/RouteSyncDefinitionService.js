
(function (appControllers) {

    "use strict";

    RouteSyncDefinitionService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function RouteSyncDefinitionService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addRouteSyncDefinition(onRouteSyncDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteSyncDefinitionAdded = onRouteSyncDefinitionAdded;
            };
            VRModalService.showModal('/Client/Modules/WhS_RouteSync/Views/RouteSyncDefinition/RouteSyncDefinitionEditor.html', null, settings);
        }

        function editRouteSyncDefinition(routeSyncDefinitionId, onRouteSyncDefinitionUpdated) {
            var settings = {};

            var parameters = {
                routeSyncDefinitionId: routeSyncDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteSyncDefinitionUpdated = onRouteSyncDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_RouteSync/Views/RouteSyncDefinition/RouteSyncDefinitionEditor.html', parameters, settings);
        }

        function getEntityUniqueName() {
            return "WhS_RouteSync_RouteSyncDefinition";
        }

        function registerObjectTrackingDrillDownToRouteSyncDefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, routeSyncDefinitionItem) {
                routeSyncDefinitionItem.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: routeSyncDefinitionItem.Entity.RouteSyncDefinitionId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return routeSyncDefinitionItem.objectTrackingGridAPI.load(query);
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
            addRouteSyncDefinition: addRouteSyncDefinition,
            editRouteSyncDefinition: editRouteSyncDefinition,
            registerObjectTrackingDrillDownToRouteSyncDefinition: registerObjectTrackingDrillDownToRouteSyncDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('WhS_RouteSync_RouteSyncDefinitionService', RouteSyncDefinitionService);

})(appControllers);