(function (appControllers) {

    "use strict";
    receivedSupplierPriceListAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SupPL_ModuleConfig", "SecurityService"];

    function receivedSupplierPriceListAPIService(BaseAPIService, UtilsService, WhS_SupPL_ModuleConfig, SecurityService) {

        var controllerName = "ReceivedSupplierPriceList";

        function GetFilteredReceivedSupplierPriceList(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, "GetFilteredReceivedSupplierPriceList"), input);
        }

        return ({
            GetFilteredReceivedSupplierPriceList: GetFilteredReceivedSupplierPriceList,
        });
    }

    appControllers.service("WhS_SupPL_SupplierPriceListAPIService", receivedSupplierPriceListAPIService);
})(appControllers);