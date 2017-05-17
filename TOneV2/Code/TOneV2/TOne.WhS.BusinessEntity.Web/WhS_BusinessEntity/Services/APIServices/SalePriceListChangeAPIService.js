(function (appControllers) {

    "use strict";
    salePriceListChangeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];
    function salePriceListChangeAPIService(baseApiService, utilsService, whSBeModuleConfig) {

        var controllerName = "SalePriceListChange";

        function GetFilteredSalePriceListCodeChanges(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredSalePriceListCodeChanges"), input);
        }
        function GetFilteredSalePriceListRateChanges(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredSalePriceListRateChanges"), input);
        }
        function GetFilteredSalePriceListRPChanges(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredSalePriceListRPChanges"), input);
        }
        function GetOwnerName(priceListId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetOwnerName"), {
                priceListId: priceListId
            });
        }
        return ({
            GetFilteredSalePriceListCodeChanges: GetFilteredSalePriceListCodeChanges,
            GetFilteredSalePriceListRateChanges: GetFilteredSalePriceListRateChanges,
            GetFilteredSalePriceListRPChanges: GetFilteredSalePriceListRPChanges,
            GetOwnerName: GetOwnerName
        });
    }

    appControllers.service("WhS_BE_SalePriceListChangeAPIService", salePriceListChangeAPIService);

})(appControllers);