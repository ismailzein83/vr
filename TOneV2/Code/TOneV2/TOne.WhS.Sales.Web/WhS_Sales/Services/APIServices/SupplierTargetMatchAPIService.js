(function (appControllers) {


    "use strict";
    countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig'];

    function countryAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {
        var controllerName = "SupplierTargetMatch";

        function GetFilteredSupplierTargetMatches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierTargetMatches"), input);
        }

        function GetTargetMatchMethods() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetTargetMatchMethods"));
        }

        function DownloadSupplierTargetMatches() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "DownloadSupplierTargetMatches"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        return ({
            GetFilteredSupplierTargetMatches: GetFilteredSupplierTargetMatches,
            GetTargetMatchMethods: GetTargetMatchMethods,
            DownloadSupplierTargetMatches: DownloadSupplierTargetMatches
        });
    }

    appControllers.service('WhS_Sales_SupplierTargetMatchAPIService', countryAPIService);

})(appControllers);