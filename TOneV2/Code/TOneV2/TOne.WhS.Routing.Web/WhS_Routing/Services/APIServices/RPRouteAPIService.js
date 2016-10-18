﻿(function (appControllers) {

    "use strict";
    rpRouteAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Routing_ModuleConfig"];

    function rpRouteAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "RPRoute";

        function GetFilteredRPRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRPRoutes"), input);
        }

        function GetRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, supplierId, currencyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRPRouteOptionSupplier"), {
                routingDatabaseId: routingDatabaseId,
                routingProductId: routingProductId,
                saleZoneId: saleZoneId,
                supplierId: supplierId,
                currencyId: currencyId
            });
        }

        function GetPoliciesOptionTemplates(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetPoliciesOptionTemplates"), {
                filter: filter
            });
        }
        
        function GetFilteredRPRouteOptions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRPRouteOptions"), input);
        }

        function GetRPSettingsAddBlockedOptions() {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRPSettingsAddBlockedOptions"));
        }

        return ({
            GetFilteredRPRoutes: GetFilteredRPRoutes,
            GetRPRouteOptionSupplier: GetRPRouteOptionSupplier,
            GetPoliciesOptionTemplates: GetPoliciesOptionTemplates,
            GetFilteredRPRouteOptions: GetFilteredRPRouteOptions,
            GetRPSettingsAddBlockedOptions: GetRPSettingsAddBlockedOptions
        });
    }

    appControllers.service("WhS_Routing_RPRouteAPIService", rpRouteAPIService);
})(appControllers);