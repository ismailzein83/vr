(function (appControllers) {
    "use strict";
    customerInfoAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function customerInfoAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "CustomerInfo";

        function GetCustomerInfo() {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCustomerInfo"));
        }

        return {
            GetCustomerInfo: GetCustomerInfo
        };
    };
    appControllers.service("Demo_Module_CustomerInfoAPIService", customerInfoAPIService);

})(appControllers);