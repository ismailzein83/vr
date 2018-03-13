(function (appControllers) {
    'use strict';

    SettingConfigsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];

    function SettingConfigsAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = 'SettingConfigs';

        function GetSettingTypeTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetSettingTypeTemplateConfigs'));
        }

        function GetDimensionsTypeTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetDimensionsTypeTemplateConfigs'));
        }

        return ({
            GetSettingTypeTemplateConfigs: GetSettingTypeTemplateConfigs,
            GetDimensionsTypeTemplateConfigs: GetDimensionsTypeTemplateConfigs
        });
    }


    appControllers.service('Demo_Module_SettingConfigsAPIService', SettingConfigsAPIService);
})(appControllers);