
(function (appControllers) {

    "use strict";

    RouteTableService.$inject = ['VRModalService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function RouteTableService(NPModalService, UtilsService, VRCommon_ObjectTrackingService) {

        function addRouteTable(onRouteTableAdded) {

            var settings = {};
            var parameters = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteTableAdded = onRouteTableAdded;

            };
            VRModalService.showModal('/Client/Modules/NP_IVSwitch/Views/RouteTable/RouteTableEditor.html', parameters, settings);
        };

        function editRouteTable(routeTableId, onRouteTableUpdated) {
            var settings = {};
            var parameters = {
                routeTableId: routeTableId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRouteTableUpdated = onRouteTableUpdated;

            };
            VRModalService.showModal('/Client/Modules/NP_IVSwitch/Views/RouteTable/RouteTableEditor.html', parameters, settings);
        };


        return {
            addRouteTable: addRouteTable,
            editRouteTable: editRouteTable
        };
    }

    appControllers.service('NP_IVSwitch_RouteTableService', RouteTableService);

})(appControllers);