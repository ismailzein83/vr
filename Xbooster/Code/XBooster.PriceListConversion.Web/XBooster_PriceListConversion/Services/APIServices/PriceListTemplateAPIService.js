(function (appControllers) {

    "use strict";
    PriceListTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'XBooster_PriceListConversion_ModuleConfig'];
    function PriceListTemplateAPIService(BaseAPIService, UtilsService, SecurityService, XBooster_PriceListConversion_ModuleConfig) {
        var controllerName = 'PriceListTemplate';

        function GetFilteredInputPriceListTemplates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'GetFilteredInputPriceListTemplates'), input);
        }

        function AddOutputPriceListTemplate(priceListTemplateObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'AddOutputPriceListTemplate'), priceListTemplateObj);
        }

        function UpdateInputPriceListTemplate(priceListTemplateObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'UpdateInputPriceListTemplate'), priceListTemplateObj);
        }

        function AddInputPriceListTemplate(priceListTemplateObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'AddInputPriceListTemplate'), priceListTemplateObj);
        }

        function UpdateOutputPriceListTemplate(priceListTemplateObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'UpdateOutputPriceListTemplate'), priceListTemplateObj);
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

        function GetOutputPriceListTemplates(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'GetOutputPriceListTemplates'),
                {
                    filter: filter
                });
        }

        function GetInputPriceListTemplates(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'GetInputPriceListTemplates'),
                {
                    filter: filter
                });
        }

        function GetInputPriceListConfigurationTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(XBooster_PriceListConversion_ModuleConfig.moduleName, controllerName, 'GetInputPriceListConfigurationTemplateConfigs'));
        }
        

        return ({
            GetFilteredInputPriceListTemplates: GetFilteredInputPriceListTemplates,
            UpdateInputPriceListTemplate: UpdateInputPriceListTemplate,
            AddInputPriceListTemplate:AddInputPriceListTemplate,
            AddOutputPriceListTemplate: AddOutputPriceListTemplate,
            UpdateOutputPriceListTemplate:UpdateOutputPriceListTemplate,
            GetOutputPriceListConfigurationTemplateConfigs: GetOutputPriceListConfigurationTemplateConfigs,
            GetPriceListTemplate: GetPriceListTemplate,
            GetOutputPriceListTemplates: GetOutputPriceListTemplates,
            GetInputPriceListConfigurationTemplateConfigs: GetInputPriceListConfigurationTemplateConfigs,
            GetInputPriceListTemplates: GetInputPriceListTemplates

        });
    }
    appControllers.service('XBooster_PriceListConversion_PriceListTemplateAPIService', PriceListTemplateAPIService);

})(appControllers);