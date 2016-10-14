(function (appControllers) {

    "use strict";
    SupplierOtherRateAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function SupplierOtherRateAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "SupplierOtherRate";

        function GetFilteredSupplierOtherRates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierOtherRates"), input);
        }


        return ({
            GetFilteredSupplierOtherRates: GetFilteredSupplierOtherRates
        });
    }

    appControllers.service("WhS_BE_SupplierOtherRateAPIService", SupplierOtherRateAPIService);
})(appControllers);