(function (appControllers) {

    "use strict";
    saleEntityZoneServiceAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function saleEntityZoneServiceAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "SaleEntityZoneService";

        function GetFilteredSaleEntityZoneServices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSaleEntityZoneServices"), input);
        }
        return ({
            GetFilteredSaleEntityZoneServices: GetFilteredSaleEntityZoneServices
        });
    }

    appControllers.service("WhS_BE_SaleEntityZoneServiceAPIService", saleEntityZoneServiceAPIService);
})(appControllers);