(function (appControllers) {

    "use strict";
    PriceListConversionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'XBooster_PriceListConversion_ModuleConfig'];
    function PriceListConversionAPIService(baseApiService, utilsService, SecurityService, moduleConfig) {
        var controllerName = 'PriceListConversion';
       
        function PriceListConvertAndDownload(priceListConversion) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "PriceListConvertAndDownload"), priceListConversion, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        return ({
            PriceListConvertAndDownload: PriceListConvertAndDownload
        });
    }
    appControllers.service('ExcelConversion_PriceListConversionAPIService', PriceListConversionAPIService);

})(appControllers);