(function (appControllers) {

    "use strict";
    PriceListConversionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'XBooster_PriceListConversion_ModuleConfig'];
    function PriceListConversionAPIService(baseApiService, utilsService, SecurityService, moduleConfig) {
        var controllerName = 'PriceListConversion';
       
        function ConvertAndDownloadPriceList(priceListConversion) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, controllerName, "ConvertAndDownloadPriceList"), priceListConversion, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        function HasConvertAndDownloadPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(moduleConfig.moduleName, controllerName, ['ConvertAndDownloadPriceList']));
        }
        return ({
            ConvertAndDownloadPriceList: ConvertAndDownloadPriceList,
            HasConvertAndDownloadPermission: HasConvertAndDownloadPermission
        });
    }
    appControllers.service('XBooster_PriceListConversion_PriceListConversionAPIService', PriceListConversionAPIService);

})(appControllers);