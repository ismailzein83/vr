(function (appControllers) {

    "use strict";
    whSJazzTaxCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzTaxCodeAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "WhSJazzTaxCode";

        function GetAllTaxCodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllTaxCodes'), {
            });
        }
        function GetTaxCodesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetTaxCodesInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllTaxCodes: GetAllTaxCodes,
            GetTaxCodesInfo: GetTaxCodesInfo
        });
    }

    appControllers.service("WhS_Jazz_TaxCodeAPIService", whSJazzTaxCodeAPIService);
})(appControllers);