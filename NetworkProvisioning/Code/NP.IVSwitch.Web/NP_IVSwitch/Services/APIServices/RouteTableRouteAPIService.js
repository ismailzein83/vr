
(function (appControllers) {

    "use strict";
    RouteTableRouteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig'];

    function RouteTableRouteAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig) {

        var controllerName = "RouteTableRoute";


        function GetFilteredRouteTableRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetFilteredRouteTableRoutes"),input);
        }

        function AddRouteTableRoutes(RouteTableRTItem) {
            
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'AddRouteTableRoutes'), RouteTableRTItem);
        }

        function UpdateRouteTableRoute(RouteTableItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'UpdateRouteTableRoute'), RouteTableItem);
        }

        function GetRouteTableRoutesOptions(routeTableId, routeTableDestination) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetRouteTableRoutesOptions"),
                {
                    routeTableId: routeTableId,
                    destination: routeTableDestination

                });
        }
        function DeleteRouteTableRoute(routeTableId, destination) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "DeleteRouteTableRoute"),
                {
                    routeTableId: routeTableId,
                    destination: destination
                });
        }


        return ({
            GetFilteredRouteTableRoutes: GetFilteredRouteTableRoutes,
            GetRouteTableRoutesOptions: GetRouteTableRoutesOptions,
            AddRouteTableRoutes: AddRouteTableRoutes,
            UpdateRouteTableRoute: UpdateRouteTableRoute,
            DeleteRouteTableRoute: DeleteRouteTableRoute


        });
    }

    appControllers.service('NP_IVSwitch_RouteTableRouteAPIService', RouteTableRouteAPIService);

})(appControllers);