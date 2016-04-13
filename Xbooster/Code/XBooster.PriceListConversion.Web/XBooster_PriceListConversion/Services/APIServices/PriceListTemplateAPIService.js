(function (appControllers) {

    "use strict";
    PriceListTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'XBooster_PriceListConversion_ModuleConfig'];
    function PriceListTemplateAPIService(BaseAPIService, UtilsService, SecurityService, XBooster_PriceListConversion_ModuleConfig) {
        var controllerName = 'PriceListTemplate';

        function GetFilteredInputPriceListTemplates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'GetFilteredInputPriceListTemplates'), input);
        }
        function AddInputPriceListTemplate(priceListTemplateObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'AddInputPriceListTemplate'), priceListTemplateObj);
        }
        function UpdateInputPriceListTemplate(priceListTemplateObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'UpdateInputPriceListTemplate'), priceListTemplateObj);
        }
        function GetOutputPriceListConfigurationTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'GetOutputPriceListConfigurationTemplateConfigs'));
        }
        function GetPriceListTemplate(priceListTemplateId) {
            return BaseAPIService.get(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'GetPriceListTemplate'),
                {
                    priceListTemplateId: priceListTemplateId
                });
        }
        return ({
            GetFilteredInputPriceListTemplates: GetFilteredInputPriceListTemplates,
            UpdateInputPriceListTemplate: UpdateInputPriceListTemplate,
            AddInputPriceListTemplate: AddInputPriceListTemplate,
            GetOutputPriceListConfigurationTemplateConfigs: GetOutputPriceListConfigurationTemplateConfigs,
            GetPriceListTemplate: GetPriceListTemplate
        });
    }
    appControllers.service('XBooster_PriceListConversion_PriceListTemplateAPIService', PriceListTemplateAPIService);

})(appControllers);