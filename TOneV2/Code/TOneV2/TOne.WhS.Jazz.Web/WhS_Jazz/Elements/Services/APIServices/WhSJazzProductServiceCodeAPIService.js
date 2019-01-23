(function (appControllers) {

    "use strict";
    whSJazzProductServiceCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzProductServiceCodeAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "WhSJazzProductServiceCode";

        function GetAllProductServiceCodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllProductServiceCodes'), {
            });
        }
        function GetProductServiceCodesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetProductServiceCodesInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllProductServiceCodes: GetAllProductServiceCodes,
            GetProductServiceCodesInfo: GetProductServiceCodesInfo
        });
    }

    appControllers.service("WhS_Jazz_ProductServiceCodeAPIService", whSJazzProductServiceCodeAPIService);
})(appControllers);