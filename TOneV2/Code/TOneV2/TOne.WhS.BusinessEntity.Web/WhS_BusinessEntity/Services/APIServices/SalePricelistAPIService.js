(function (appControllers) {

    "use strict";
    salePricelistAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];
    function salePricelistAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "SalePricelist";

        function GetFilteredSalePriceLists(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSalePriceLists"), input);
        }
        function SendPriceList(priceListId) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "SendPriceList"), priceListId);
        }

        return ({
            GetFilteredSalePriceLists: GetFilteredSalePriceLists,
            SendPriceList: SendPriceList
        });
    }

    appControllers.service("WhS_BE_SalePricelistAPIService", salePricelistAPIService);

})(appControllers);