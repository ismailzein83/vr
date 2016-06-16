(function (appControllers) {

    "use strict";
    SwitchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function SwitchAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "Switch";

        function GetSwitchSettingsTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetSwitchSettingsTemplateConfigs"));

        }

        return ({
            GetSwitchSettingsTemplateConfigs: GetSwitchSettingsTemplateConfigs
        });
    }

    appControllers.service('Retail_BE_SwitchAPIService', SwitchAPIService);

})(appControllers);