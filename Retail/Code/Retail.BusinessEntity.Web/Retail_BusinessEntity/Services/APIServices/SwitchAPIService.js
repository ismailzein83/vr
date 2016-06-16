(function (appControllers) {

    "use strict";
    SwitchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function SwitchAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "Switch";

        function GetSwitchSettingsTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetSwitchSettingsTemplateConfigs"));

        }

        function GetFilteredSwitches(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredSwitches'), input);
        }

        return ({
            GetSwitchSettingsTemplateConfigs: GetSwitchSettingsTemplateConfigs,
            GetFilteredSwitches: GetFilteredSwitches
        });
    }

    appControllers.service('Retail_BE_SwitchAPIService', SwitchAPIService);

})(appControllers);