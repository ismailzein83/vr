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
        function CheckIfAnyPriceListExists(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "CheckIfAnyPriceListExists"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }
        function SendCustomerPriceLists(customerPriceListEmailInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "SendCustomerPriceLists"), customerPriceListEmailInput);
        }
        function SendPricelistsWithCheckNotSendPreviousPricelists(sendPricelistsWithCheckPreviousInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "SendPricelistsWithCheckNotSendPreviousPricelists"), sendPricelistsWithCheckPreviousInput);
        }
        function CheckIfCustomerHasNotSendPricelist(pricelistId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "CheckIfCustomerHasNotSendPricelist"), {
                pricelistId: pricelistId
            });
        }

        return {
            GetFilteredSalePriceLists: GetFilteredSalePriceLists,
            SendPriceList: SendPriceList,
            GetSalePriceListIdsByProcessInstanceId: GetSalePriceListIdsByProcessInstanceId,
            SendCustomerPriceLists: SendCustomerPriceLists,
            CheckIfAnyPriceListExists: CheckIfAnyPriceListExists,
            SendPricelistsWithCheckNotSendPreviousPricelists: SendPricelistsWithCheckNotSendPreviousPricelists,
            CheckIfCustomerHasNotSendPricelist: CheckIfCustomerHasNotSendPricelist
        };
    }

    appControllers.service("WhS_BE_SalePricelistAPIService", salePricelistAPIService);

})(appControllers);