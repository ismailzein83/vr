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

        function GetSalePriceListIdsByProcessInstanceId(processInstanceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSalePriceListIdsByProcessInstanceId"), {
                processInstanceId: processInstanceId
            });
        }

        function SendCustomerPriceLists(customerPriceListIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "SendCustomerPriceLists"), customerPriceListIds);
        }

        return {
            GetFilteredSalePriceLists: GetFilteredSalePriceLists,
            SendPriceList: SendPriceList,
            GetSalePriceListIdsByProcessInstanceId: GetSalePriceListIdsByProcessInstanceId,
            SendCustomerPriceLists: SendCustomerPriceLists
        };
    }

    appControllers.service("WhS_BE_SalePricelistAPIService", salePricelistAPIService);

})(appControllers);