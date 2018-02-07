(function (appControllers) {

    "use strict";

    customerRouteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function customerRouteAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "CustomerRoute";

        function GetFilteredCustomerRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetFilteredCustomerRoutes"), input);
        };

        return ({
            GetFilteredCustomerRoutes: GetFilteredCustomerRoutes
        });
    }

    appControllers.service('WhS_Routing_CustomerRouteAPIService', customerRouteAPIService);
})(appControllers);