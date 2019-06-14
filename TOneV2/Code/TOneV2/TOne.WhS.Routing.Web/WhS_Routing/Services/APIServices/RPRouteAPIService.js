﻿(function (appControllers) {

    "use strict";

    rpRouteAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Routing_ModuleConfig"];

    function rpRouteAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "RPRoute";

        function GetFilteredRPRoutesByZone(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRPRoutesByZone"), input);
        }

        function GetFilteredRPRoutesByCode(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRPRoutesByCode"), input);
        }

        function GetRPRouteOptionSupplier(rpRouteOptionSupplierInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetRPRouteOptionSupplier"), rpRouteOptionSupplierInput);
        }

        function GetPoliciesOptionTemplates(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetPoliciesOptionTemplates"), {
                filter: filter
            });
        }
        
        function GetFilteredRPRouteOptions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredRPRouteOptions"), input);
        }

        return ({
            GetFilteredRPRoutesByZone: GetFilteredRPRoutesByZone,
            GetFilteredRPRoutesByCode: GetFilteredRPRoutesByCode,
            GetRPRouteOptionSupplier: GetRPRouteOptionSupplier,
            GetPoliciesOptionTemplates: GetPoliciesOptionTemplates,
            GetFilteredRPRouteOptions: GetFilteredRPRouteOptions
        });
    }

    appControllers.service("WhS_Routing_RPRouteAPIService", rpRouteAPIService);
})(appControllers);