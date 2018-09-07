
(function (appControllers) {

    "use strict";

    RouteTableService.$inject = ['VRModalService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function RouteTableService(NPModalService, UtilsService, VRCommon_ObjectTrackingService) {
        function addRouteTable(onRouteTableAdded) {
            var settings = {};
            var parameters = { RouteTableViewType: onRouteTableAdded.RouteTableViewType };
            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteTableAdded = onRouteTableAdded.onRouteTableAdded;

            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/RouteTable/RouteTableEditor.html', parameters, settings);
        };

        function editRouteTable(routeTable, onRouteTableUpdated) {
            var settings = {};
            var parameters = routeTable;
            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteTableUpdated = onRouteTableUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/RouteTable/RouteTableEditor.html', parameters, settings);
        };

        return {
            addRouteTable: addRouteTable,
            editRouteTable: editRouteTable
        };
    }

    appControllers.service('NP_IVSwitch_RouteTableService', RouteTableService);

})(appControllers);