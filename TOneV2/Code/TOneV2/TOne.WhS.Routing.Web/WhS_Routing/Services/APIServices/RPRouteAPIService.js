(function (appControllers) {

    "use strict";
    rpRouteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function rpRouteAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        function GetFilteredRPRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RPRoute", "GetFilteredRPRoutes"), input);
        }

        function GetRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, supplierId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RPRoute", "GetRPRouteOptionSupplier"), {
                routingDatabaseId: routingDatabaseId,
                routingProductId: routingProductId,
                saleZoneId: saleZoneId,
                supplierId: supplierId
            });
        }

        function GetPoliciesOptionTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RPRoute", "GetPoliciesOptionTemplates"));
        }
        
        function GetRouteOptionDetails(routingDatabaseId, policyOptionConfigId, routingProductId, saleZoneId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RPRoute", "GetRouteOptionDetails"),
                {
                    routingDatabaseId: routingDatabaseId,
                    policyOptionConfigId: policyOptionConfigId,
                    routingProductId: routingProductId,
                    saleZoneId: saleZoneId
                });
        }

        return ({
            GetFilteredRPRoutes: GetFilteredRPRoutes,
            GetRPRouteOptionSupplier: GetRPRouteOptionSupplier,
            GetPoliciesOptionTemplates: GetPoliciesOptionTemplates,
            GetRouteOptionDetails: GetRouteOptionDetails
        });
    }

    appControllers.service('WhS_Routing_RPRouteAPIService', rpRouteAPIService);
})(appControllers);