(function (appControllers) {

    "use strict";
    PriceListConversionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'ExcelConversion_ModuleConfig'];
    function PriceListConversionAPIService(baseApiService, utilsService, SecurityService, moduleConfig) {
        var controllerName = 'PriceListConversion';
       
        function PriceListConvertAndDownload(priceListConversion) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "PriceListConvertAndDownload"), priceListConversion, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        function ConvertAndDownload(excelToConvert) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "ConvertAndDownload"), excelToConvert, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        return ({
            PriceListConvertAndDownload: PriceListConvertAndDownload,
            ConvertAndDownload: ConvertAndDownload
        });
    }
    appControllers.service('ExcelConversion_PriceListConversionAPIService', PriceListConversionAPIService);

})(appControllers);