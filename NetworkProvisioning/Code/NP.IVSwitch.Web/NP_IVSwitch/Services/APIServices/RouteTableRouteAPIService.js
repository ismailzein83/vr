
(function (appControllers) {

    "use strict";
    RouteTableRouteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function RouteTableRouteAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "RouteTableRoute";


        function GetFilteredRouteTableRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetFilteredRouteTableRoutes"),input);
        }

        function HasViewRouteTableRoutePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['GetFilteredRouteTableRoutes']));
        }

        function AddRouteTableRoutes(RouteTableRTItem) {
            
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'AddRouteTableRoutes'), RouteTableRTItem);
        }
        function HasAddRouteTableRoutesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['AddRouteTableRoutes']));
        }

        function UpdateRouteTableRoute(RouteTableItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'UpdateRouteTableRoute'), RouteTableItem);
        }
        function HasUpdateRouteTableRoutePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['UpdateRouteTableRoute']));
        }
        function DeleteRouteTableRoute(routeTableId, destination) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "DeleteRouteTableRoute"),
                {
                    routeTableId: routeTableId,
                    destination: destination
                });
        }
        function HasDeleteRouteTableRoutePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['DeleteRouteTableRoute']));
        }
        function GetRouteTableRoutesOptions(routeTableId, routeTableDestination) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetRouteTableRoutesOptions"),
                {
                    routeTableId: routeTableId,
                    destination: routeTableDestination

                });
        }



        return ({
            GetFilteredRouteTableRoutes: GetFilteredRouteTableRoutes,
            GetRouteTableRoutesOptions: GetRouteTableRoutesOptions,
            AddRouteTableRoutes: AddRouteTableRoutes,
            HasAddRouteTableRoutesPermission: HasAddRouteTableRoutesPermission,
            UpdateRouteTableRoute: UpdateRouteTableRoute,
            HasUpdateRouteTableRoutePermission: HasUpdateRouteTableRoutePermission,
            DeleteRouteTableRoute: DeleteRouteTableRoute,
            HasDeleteRouteTableRoutePermission: HasDeleteRouteTableRoutePermission,
            HasViewRouteTableRoutePermission: HasViewRouteTableRoutePermission



        });
    }

    appControllers.service('NP_IVSwitch_RouteTableRouteAPIService', RouteTableRouteAPIService);

})(appControllers);