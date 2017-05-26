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
        function DownloadSalePriceList(pricelistId, priceListType) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "DownloadSalePriceList"), { salepriceListId: pricelistId, salePriceListType: priceListType }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        function GenerateSalePriceListFile(pricelistId, priceListType) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GenerateSalePriceListFile"), { salepriceListId: pricelistId, salePriceListType: priceListType }
            );
        }
        function EvaluateSalePriceListEmail(pricelistId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "EvaluateSalePriceListEmail"), { salepriceListId: pricelistId });
        }
        function GetOwnerPriceListType(priceListId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetOwnerPriceListType"), {
                priceListId: priceListId
            });
        }
        return ({
            GetFilteredSalePriceListCodeChanges: GetFilteredSalePriceListCodeChanges,
            GetFilteredSalePriceListRateChanges: GetFilteredSalePriceListRateChanges,
            GetFilteredSalePriceListRPChanges: GetFilteredSalePriceListRPChanges,
            GetOwnerName: GetOwnerName,
            DownloadSalePriceList: DownloadSalePriceList,
            GenerateSalePriceListFile: GenerateSalePriceListFile,
            EvaluateSalePriceListEmail: EvaluateSalePriceListEmail,
            GetOwnerPriceListType: GetOwnerPriceListType
        });
    }

    appControllers.service("WhS_BE_SalePriceListChangeAPIService", salePriceListChangeAPIService);

})(appControllers);