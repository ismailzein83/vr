(function (appControllers) {

    "use strict";
    salePriceListChangeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];
    function salePriceListChangeAPIService(baseApiService, utilsService, whSBeModuleConfig) {

        var controllerName = "SalePriceListChange";
        
        function GetFilteredSalePriceListCodeChanges(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredSalePriceListCodeChanges"), input);
        }
        function DoCustomerTemporaryPricelistsExists(processInstanceId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "DoCustomerTemporaryPricelistsExists"), { processInstanceId: processInstanceId });
        }
        function GetFilteredTemporarySalePriceLists(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredTemporarySalePriceLists"), input);
        }
        function GetFilteredSalePriceListRateChanges(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredSalePriceListRateChanges"), input);
        }
        function GetSalePricelistCodes(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetSalePricelistCodes"), input);
        }
        function GetFilteredSalePriceListRPChanges(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredSalePriceListRPChanges"), input);
        }
        function GetOwnerOptions(priceListId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetOwnerOptions"), {
                priceListId: priceListId
            });
        }

        function GetPricelistSalePricelistVRFile(pricelistInput) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetPricelistSalePricelistVRFile"), {
                salepriceListId: pricelistInput.PriceListId,
                salePriceListType: pricelistInput.PriceListTypeId,
                salePriceListTemplateId: pricelistInput.PricelistTemplateId
            });
        }
        function DownloadSalePriceList(fileId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "DownloadSalePriceList"), { fileId: fileId },
            {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        function GenerateAndEvaluateSalePriceListEmail(pricelistInput) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GenerateAndEvaluateSalePriceListEmail"), pricelistInput);
        }
        function GenerateAndEvaluateSalePricelistEmailByPricelistIdAndOwnerId(pricelistId,ownerId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GenerateAndEvaluateSalePricelistEmailByPricelistIdAndOwnerId"), {
                pricelistId: pricelistId,
                ownerId: ownerId
            });
        }
        function GetOwnerPriceListType(priceListId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetOwnerPriceListType"), {
                priceListId: priceListId
            });
        }
        function GetOwnerPricelistTemplateId(priceListId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetOwnerPricelistTemplateId"), {
                priceListId: priceListId
            });
        }
        function SetPriceListAsSent(priceListId) {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "SetPriceListAsSent"), {
                priceListId: priceListId
            });
        }

        function GetFilteredCustomerRatePreviews(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, 'GetFilteredCustomerRatePreviews'), input);
        }
        function GetFilteredRoutingProductPreviews(input) {

            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, 'GetFilteredRoutingProductPreviews'), input);
        }
        return ({
            GetFilteredSalePriceListCodeChanges: GetFilteredSalePriceListCodeChanges,
            GetFilteredSalePriceListRateChanges: GetFilteredSalePriceListRateChanges,
            GetFilteredSalePriceListRPChanges: GetFilteredSalePriceListRPChanges,
            GetOwnerOptions: GetOwnerOptions,
            GenerateAndEvaluateSalePricelistEmailByPricelistIdAndOwnerId:GenerateAndEvaluateSalePricelistEmailByPricelistIdAndOwnerId,
            GetPricelistSalePricelistVRFile: GetPricelistSalePricelistVRFile,
            DownloadSalePriceList: DownloadSalePriceList,
            GenerateAndEvaluateSalePriceListEmail: GenerateAndEvaluateSalePriceListEmail,
            GetOwnerPriceListType: GetOwnerPriceListType,
            SetPriceListAsSent: SetPriceListAsSent,
            GetOwnerPricelistTemplateId: GetOwnerPricelistTemplateId,
            GetFilteredCustomerRatePreviews: GetFilteredCustomerRatePreviews,
            GetFilteredRoutingProductPreviews: GetFilteredRoutingProductPreviews,
            GetSalePricelistCodes: GetSalePricelistCodes,
            GetFilteredTemporarySalePriceLists: GetFilteredTemporarySalePriceLists,
            DoCustomerTemporaryPricelistsExists: DoCustomerTemporaryPricelistsExists
        });
    }

    appControllers.service("WhS_BE_SalePriceListChangeAPIService", salePriceListChangeAPIService);

})(appControllers);