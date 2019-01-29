(function (appControllers) {

    "use strict";
    whSJazzCustomerTypeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];
    
    function whSJazzCustomerTypeAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "CustomerType";

        function GetAllCustomerTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllCustomerTypes'), {
            });
        }
        function GetCustomerTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetCustomerTypesInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllCustomerTypes: GetAllCustomerTypes,
            GetCustomerTypesInfo: GetCustomerTypesInfo
        });
    }

    appControllers.service("WhS_Jazz_CustomerTypeAPIService", whSJazzCustomerTypeAPIService);
})(appControllers);