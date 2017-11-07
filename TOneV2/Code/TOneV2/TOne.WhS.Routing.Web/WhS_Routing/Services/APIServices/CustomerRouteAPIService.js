(function (appControllers) {

    "use strict";
    customerRouteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function customerRouteAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "CustomerRoute";

        function GetFilteredCustomerRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredCustomerRoutes"), input);
        }

        function GetUpdatedCustomerRoutes(customerRouteDefinitions) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetUpdatedCustomerRoutes"), customerRouteDefinitions);
        }

        return ({
            GetFilteredCustomerRoutes: GetFilteredCustomerRoutes,
            GetUpdatedCustomerRoutes: GetUpdatedCustomerRoutes
        });
    }

    appControllers.service('WhS_Routing_CustomerRouteAPIService', customerRouteAPIService);
})(appControllers);