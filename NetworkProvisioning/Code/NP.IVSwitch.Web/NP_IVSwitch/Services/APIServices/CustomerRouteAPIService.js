
(function (appControllers) {

    "use strict";
    CustomerRouteAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function CustomerRouteAPIService(baseApiService, utilsService, npIvSwitchModuleConfig) {

        var controllerName = "CustomerRoute";

        function GetFilteredCustomerRoutes(input) {
            return baseApiService.post(utilsService.getServiceURL(npIvSwitchModuleConfig.moduleName, controllerName, 'GetFilteredCustomerRoutes'), input);
        }

        return ({
            GetFilteredCustomerRoutes: GetFilteredCustomerRoutes

        });
    }

    appControllers.service('NP_IVSwitch_CustomerRouteAPIService', CustomerRouteAPIService);

})(appControllers);