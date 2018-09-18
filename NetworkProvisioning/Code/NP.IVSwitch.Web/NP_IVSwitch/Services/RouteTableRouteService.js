
(function (appControllers) {

    "use strict";

    RouteTableRouteService.$inject = ['VRModalService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function RouteTableRouteService(NPModalService, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addRouteTableRoutes(onRouteTableRTAdded, payloadForAdd) {

            var settings = {};
            var parameters = {};
            parameters = { RouteTableId: payloadForAdd.RouteTableId, RouteTableViewType: payloadForAdd.RouteTableViewType };
            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteTableRTAdded = onRouteTableRTAdded;

            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/RouteTableRT/RouteTableRouteEditor.html', parameters, settings);
        }

        function editRouteTableRoutes(routeTableItem, onRouteTableUpdated) {
            var settings = {};
            var parameters = routeTableItem;
            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteTableUpdated = onRouteTableUpdated;

            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/RouteTableRT/RouteTableRouteEditor.html', parameters, settings);
        }

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            addRouteTableRoutes: addRouteTableRoutes,
            editRouteTableRoutes: editRouteTableRoutes
        };
    }

    appControllers.service('NP_IVSwitch_RouteTableRouteService', RouteTableRouteService);

})(appControllers);