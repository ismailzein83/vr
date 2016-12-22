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

        function GetSwitch(switchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetSwitch'), {
                switchId: switchId
            });
        }

        function AddSwitch(switchItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddSwitch'), switchItem);
        }
        function HasAddSwitchPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddSwitch']));

        }
        function UpdateSwitch(switchItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateSwitch'), switchItem);
        }

        function HasUpdateSwitchPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateSwitch']));

        }

        return ({
            GetSwitchSettingsTemplateConfigs: GetSwitchSettingsTemplateConfigs,
            GetFilteredSwitches: GetFilteredSwitches,
            GetSwitch: GetSwitch,
            AddSwitch: AddSwitch,
            HasAddSwitchPermission:HasAddSwitchPermission,
            UpdateSwitch: UpdateSwitch,
            HasUpdateSwitchPermission: HasUpdateSwitchPermission
        });
    }

    appControllers.service('Retail_BE_SwitchAPIService', SwitchAPIService);

})(appControllers);