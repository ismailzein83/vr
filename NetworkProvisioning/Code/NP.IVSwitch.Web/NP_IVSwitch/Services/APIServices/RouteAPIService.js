
(function (appControllers) {

    "use strict";
    RouteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function RouteAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "Route";


        function GetFilteredRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetFilteredRoutes'), input);
        }

        function GetRoute(RouteId) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetRoute'), {
                RouteId: RouteId
            });
        }
        function GetRoutesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetRoutesInfo'), {
                filter: filter
            });
        }
        function AddRoute(RouteItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'AddRoute'), RouteItem);
        }

        function UpdateRoute(RouteItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'UpdateRoute'), RouteItem);
        }


        function HasAddRoutePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['AddRoute']));
        }

        function HasEditRoutePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['UpdateRoute']));
        }

        function GetSwitchDateTime() {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetSwitchDateTime'));
        }

        function GetRouteHistoryDetailbyHistoryId(routeHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetRouteHistoryDetailbyHistoryId'), {
                routeHistoryId: routeHistoryId
            });
        }

        return ({
            GetFilteredRoutes: GetFilteredRoutes,
            GetRoute: GetRoute,
            AddRoute: AddRoute,
            UpdateRoute: UpdateRoute,
            HasAddRoutePermission: HasAddRoutePermission,
            HasEditRoutePermission: HasEditRoutePermission,
            GetSwitchDateTime: GetSwitchDateTime,
            GetRoutesInfo: GetRoutesInfo,
            GetRouteHistoryDetailbyHistoryId: GetRouteHistoryDetailbyHistoryId
        });
    }

    appControllers.service('NP_IVSwitch_RouteAPIService', RouteAPIService);

})(appControllers);