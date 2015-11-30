(function (appControllers) {

    "use strict";
    rpRouteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function rpRouteAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        function GetFilteredRPRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RPRoute", "GetFilteredRPRoutes"), input);
        }

        function GetRPRouteOptionSupplier(routingProductId, saleZoneId, supplierId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RPRoute", "GetRPRouteOptionSupplier"), {
                routingProductId: routingProductId,
                saleZoneId: saleZoneId,
                supplierId: supplierId
            });
        }

        return ({
            GetFilteredRPRoutes: GetFilteredRPRoutes,
            GetRPRouteOptionSupplier: GetRPRouteOptionSupplier
        });
    }

    appControllers.service('WhS_Routing_RPRouteAPIService', rpRouteAPIService);
})(appControllers);