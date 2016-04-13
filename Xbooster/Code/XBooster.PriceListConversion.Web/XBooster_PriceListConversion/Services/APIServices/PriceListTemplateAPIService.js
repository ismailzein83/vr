(function (appControllers) {

    "use strict";
    PriceListTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'XBooster_PriceListConversion_ModuleConfig'];
    function PriceListTemplateAPIService(BaseAPIService, UtilsService, SecurityService, XBooster_PriceListConversion_ModuleConfig) {
        var controllerName = 'PriceListTemplate';

        function GetFilteredPriceListTemplates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'GetFilteredPriceListTemplates'), input);
        }

        return ({
            GetFilteredPriceListTemplates: GetFilteredPriceListTemplates
        });
    }
    appControllers.service('XBooster_PriceListConversion_PriceListTemplateAPIService', PriceListTemplateAPIService);

})(appControllers);