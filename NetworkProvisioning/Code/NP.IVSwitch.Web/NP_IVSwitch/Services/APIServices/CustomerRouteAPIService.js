
(function (appControllers) {

    "use strict";
    CustomerRouteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function CustomerRouteAPIService(baseApiService, utilsService, npIvSwitchModuleConfig) {

        var controllerName = "CustomerRoute";

        function GetFilteredCustomerRoutes(input) {
            return baseApiService.post(utilsService.getServiceURL(npIvSwitchModuleConfig.moduleName, controllerName, 'GetFilteredCustomerRoutes'), input);
        }
        function GetFilteredCustomerRouteOptions(input) {
            return baseApiService.post(utilsService.getServiceURL(npIvSwitchModuleConfig.moduleName, controllerName, 'GetFilteredCustomerRouteOptions'), input);
        }

        return ({
            GetFilteredCustomerRoutes: GetFilteredCustomerRoutes,
            GetFilteredCustomerRouteOptions: GetFilteredCustomerRouteOptions

        });
    }

    appControllers.service('NP_IVSwitch_CustomerRouteAPIService', CustomerRouteAPIService);

})(appControllers);