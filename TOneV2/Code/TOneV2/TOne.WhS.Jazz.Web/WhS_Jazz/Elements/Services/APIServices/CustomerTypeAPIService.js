(function (appControllers) {

    "use strict";
    whSJazzCustomerTypeCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzCustomerTypeCodeAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "Customer";

        function GetAllCustomerTypeCodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllCustomerTypeCodes'), {
            });
        }
        function GetCustomerTypeCodesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetCustomerTypeCodesInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllCustomerTypeCodes: GetAllCustomerTypeCodes,
            GetCustomerTypeCodesInfo: GetCustomerTypeCodesInfo
        });
    }

    appControllers.service("WhS_Jazz_CustomerTypeCodeAPIService", whSJazzCustomerTypeCodeAPIService);
})(appControllers);