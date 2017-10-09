(function (appControllers) {

    "use strict";

    PricingTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig', 'SecurityService'];

    function PricingTemplateAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig, SecurityService) {

        var controllerName = "PricingTemplate";

        function GetFilteredPricingTemplates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, 'GetFilteredPricingTemplates'), input);
        }

        function GetPricingTemplateEditorRuntime(pricingTemplateId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, 'GetPricingTemplateEditorRuntime'), {
                PricingTemplateId: pricingTemplateId
            });
        }

        function AddPricingTemplate(pricingTemplateItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, 'AddPricingTemplate'), pricingTemplateItem);
        }

        function UpdatePricingTemplate(pricingTemplateItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, 'UpdatePricingTemplate'), pricingTemplateItem);
        }

        function GetMarginRateCalculationExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetMarginRateCalculationExtensionConfigs"));
        }

        function HasAddPricingTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Sales_ModuleConfig.moduleName, controllerName, ['AddPricingTemplate']));
        }

        function HasUpdatePricingTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Sales_ModuleConfig.moduleName, controllerName, ['UpdatePricingTemplate']));
        }


        return ({
            GetFilteredPricingTemplates: GetFilteredPricingTemplates,
            GetPricingTemplateEditorRuntime: GetPricingTemplateEditorRuntime,
            AddPricingTemplate: AddPricingTemplate,
            UpdatePricingTemplate: UpdatePricingTemplate,
            GetMarginRateCalculationExtensionConfigs: GetMarginRateCalculationExtensionConfigs,
            HasAddPricingTemplatePermission: HasAddPricingTemplatePermission,
            HasUpdatePricingTemplatePermission: HasUpdatePricingTemplatePermission
        });
    }

    appControllers.service('WhS_Sales_PricingTemplateAPIService', PricingTemplateAPIService);

})(appControllers);