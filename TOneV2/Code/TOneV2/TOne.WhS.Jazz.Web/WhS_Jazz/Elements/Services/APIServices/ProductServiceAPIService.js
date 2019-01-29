(function (appControllers) {

    "use strict";
    whSJazzProductServiceAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzProductServiceAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "ProductService";

        function GetAllProductServices() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllProductServices'), {
            });
        }
        function GetProductServicesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetProductServicesInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllProductServices: GetAllProductServices,
            GetProductServicesInfo: GetProductServicesInfo
        });
    }

    appControllers.service("WhS_Jazz_ProductServiceAPIService", whSJazzProductServiceAPIService);
})(appControllers);