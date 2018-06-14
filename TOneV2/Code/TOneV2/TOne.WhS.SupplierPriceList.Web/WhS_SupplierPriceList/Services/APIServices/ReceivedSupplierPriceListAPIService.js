(function (appControllers) {

    "use strict";
    receivedSupplierPricelistAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SupPL_ModuleConfig", "SecurityService"];

    function receivedSupplierPricelistAPIService(BaseAPIService, UtilsService, WhS_SupPL_ModuleConfig, SecurityService) {

        var controllerName = "ReceivedSupplierPriceList";

        function GetFilteredReceivedSupplierPricelist(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, "GetFilteredReceivedSupplierPriceList"), input);
        }

        return ({
            GetFilteredReceivedSupplierPricelist: GetFilteredReceivedSupplierPricelist,
        });
    }

    appControllers.service("WhS_SupPL_ReceivedSupplierPricelistAPIService", receivedSupplierPricelistAPIService);
})(appControllers);