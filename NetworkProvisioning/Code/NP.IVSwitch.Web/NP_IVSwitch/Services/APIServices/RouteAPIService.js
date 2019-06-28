
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
		function DeleteRoute(routeId) {
			return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'DeleteRoute'), {
				routeId: routeId,
			});
		}
		function BlockRoute(routeId) {
			return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'BlockRoute'), {
				routeId: routeId,
			});
		}

		function HasBlockRoutePermission() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['BlockRoute']));
		}

		function InActivateRoute(routeId) {
			return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'InActivateRoute'), {
				routeId: routeId,
			});
		} 

		function HasInActivateRoutePermission() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['InActivateRoute']));
		}

		function ActivateRoute(routeId) {
			return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'ActivateRoute'), {
				routeId: routeId,
			});
		} 

		function HasActivateRoutePermission() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['ActivateRoute']));
		}

		function HasDeleteRoutePermission() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['DeleteRoute']));
		}
		function HasViewRoutePermission() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['GetFilteredRoutes']));
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
			GetRouteHistoryDetailbyHistoryId: GetRouteHistoryDetailbyHistoryId,
			DeleteRoute: DeleteRoute,
			BlockRoute: BlockRoute,
			InActivateRoute: InActivateRoute,
			ActivateRoute: ActivateRoute,
			HasDeleteRoutePermission: HasDeleteRoutePermission,
			HasViewRoutePermission: HasViewRoutePermission,
			HasActivateRoutePermission: HasActivateRoutePermission,
			HasInActivateRoutePermission: HasInActivateRoutePermission,
			HasBlockRoutePermission: HasBlockRoutePermission
        });
    }

    appControllers.service('NP_IVSwitch_RouteAPIService', RouteAPIService);

})(appControllers);