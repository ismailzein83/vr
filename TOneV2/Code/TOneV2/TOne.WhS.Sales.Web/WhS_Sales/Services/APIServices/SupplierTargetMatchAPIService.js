(function (appControllers) {


    "use strict";
    countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig'];

    function countryAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {
        var controllerName = "SupplierTargetMatch";

        function GetFilteredSupplierTargetMatches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierTargetMatches"), input);
        }


        return ({
            GetFilteredSupplierTargetMatches: GetFilteredSupplierTargetMatches
        });
    }

    appControllers.service('WhS_Sales_SupplierTargetMatchAPIService', countryAPIService);

})(appControllers);