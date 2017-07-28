﻿(function (appControllers) {

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
        function GetOwnerOptions(priceListId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetOwnerOptions"), {
                priceListId: priceListId
            });
        }

        function DownloadSalePriceList(pricelistId, priceListType) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "DownloadSalePriceList"), { salepriceListId: pricelistId, salePriceListType: priceListType }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        function GenerateAndEvaluateSalePriceListEmail(pricelisInput) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GenerateAndEvaluateSalePriceListEmail"), pricelisInput);
        }
        function GetOwnerPriceListType(priceListId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetOwnerPriceListType"), {
                priceListId: priceListId
            });
        }
        function SetPriceListAsSent(priceListId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "SetPriceListAsSent"), {
                priceListId: priceListId
            });
        }
        return ({
            GetFilteredSalePriceListCodeChanges: GetFilteredSalePriceListCodeChanges,
            GetFilteredSalePriceListRateChanges: GetFilteredSalePriceListRateChanges,
            GetFilteredSalePriceListRPChanges: GetFilteredSalePriceListRPChanges,
            GetOwnerOptions: GetOwnerOptions,
            DownloadSalePriceList: DownloadSalePriceList,
            GenerateAndEvaluateSalePriceListEmail: GenerateAndEvaluateSalePriceListEmail,
            GetOwnerPriceListType: GetOwnerPriceListType,
            SetPriceListAsSent: SetPriceListAsSent
        });
    }

    appControllers.service("WhS_BE_SalePriceListChangeAPIService", salePriceListChangeAPIService);

})(appControllers);