(function (appControllers) {

    "use strict";
    PriceListTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'WhS_SupPL_ModuleConfig'];
    function PriceListTemplateAPIService(BaseAPIService, UtilsService, SecurityService, WhS_SupPL_ModuleConfig) {
        var controllerName = 'PriceListTemplate';

       
        function UpdateInputPriceListTemplate(priceListTemplateObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'UpdateInputPriceListTemplate'), priceListTemplateObj);
        }

        function AddInputPriceListTemplate(priceListTemplateObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'AddInputPriceListTemplate'), priceListTemplateObj);
        }

        function GetPriceListTemplate(priceListTemplateId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'GetPriceListTemplate'),
                {
                    priceListTemplateId: priceListTemplateId
                });
        }

        function GetInputPriceListTemplates(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'GetInputPriceListTemplates'),
                {
                    filter: filter
                });
        }
        function GetInputPriceListConfigurationTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'GetInputPriceListConfigurationTemplateConfigs'));
        }

        function HasaddInputPriceListTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SupPL_ModuleConfig.moduleName, controllerName, ['AddInputPriceListTemplate']));
        }

        function HasUpdateInputPriceListTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SupPL_ModuleConfig.moduleName, controllerName, ['UpdateInputPriceListTemplate']));
        }

        function HassaveInputConfigurationPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(moduleConfig.moduleName, controllerName, ['UpdateInputPriceListTemplate', 'AddInputPriceListTemplate']));
        }

        return ({
            UpdateInputPriceListTemplate: UpdateInputPriceListTemplate,
            AddInputPriceListTemplate: AddInputPriceListTemplate,
            GetPriceListTemplate: GetPriceListTemplate,
            GetInputPriceListConfigurationTemplateConfigs: GetInputPriceListConfigurationTemplateConfigs,
            GetInputPriceListTemplates: GetInputPriceListTemplates,
            HassaveInputConfigurationPermission: HassaveInputConfigurationPermission,
            HasaddInputPriceListTemplatePermission: HasaddInputPriceListTemplatePermission,
            HasUpdateInputPriceListTemplatePermission: HasUpdateInputPriceListTemplatePermission
        });
    }
    appControllers.service('WhS_SupPL_PriceListTemplateAPIService', PriceListTemplateAPIService);

})(appControllers);