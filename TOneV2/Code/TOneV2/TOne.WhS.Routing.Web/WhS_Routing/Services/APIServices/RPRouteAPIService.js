﻿(function (appControllers) {

    "use strict";
    rpRouteAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Routing_ModuleConfig"];

    function rpRouteAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "RPRoute";

        function GetFilteredRPRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRPRoutes"), input);
        }

        function GetRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, supplierId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRPRouteOptionSupplier"), {
                routingDatabaseId: routingDatabaseId,
                routingProductId: routingProductId,
                saleZoneId: saleZoneId,
                supplierId: supplierId
            });
        }

        function GetPoliciesOptionTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetPoliciesOptionTemplates"));
        }
        
        function GetFilteredRPRouteOptions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRPRouteOptions"), input);
        }

        return ({
            GetFilteredRPRoutes: GetFilteredRPRoutes,
            GetRPRouteOptionSupplier: GetRPRouteOptionSupplier,
            GetPoliciesOptionTemplates: GetPoliciesOptionTemplates,
            GetFilteredRPRouteOptions: GetFilteredRPRouteOptions
        });
    }

    appControllers.service("WhS_Routing_RPRouteAPIService", rpRouteAPIService);
})(appControllers);