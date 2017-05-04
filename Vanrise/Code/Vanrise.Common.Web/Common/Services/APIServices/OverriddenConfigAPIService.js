(function (appControllers) {

    "use strict";
    overriddenConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function overriddenConfigAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'OverriddenConfiguration';

        function GetFilteredOverriddenConfigurations(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredOverriddenConfigurations"), input);
        }

        function GetOverriddenConfiguration(overriddenConfigurationId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetOverriddenConfiguration"), {
                overriddenConfigurationId: overriddenConfigurationId
            });
        }

        function UpdateOverriddenConfiguration(overriddenConfigObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateOverriddenConfiguration"), overriddenConfigObject);
        }

        function AddOverriddenConfiguration(overriddenConfigObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddOverriddenConfiguration"), overriddenConfigObject);
        }

        function GetOverriddenConfigurationHistoryDetailbyHistoryId(overriddenConfigurationhistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetOverriddenConfigurationHistoryDetailbyHistoryId'), {
                overriddenConfigurationhistoryId: overriddenConfigurationhistoryId
            });
        }
        function GetOverriddenConfigSettingConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetOverriddenConfigSettingConfigs'));
        }
        return ({
            GetFilteredOverriddenConfigurations: GetFilteredOverriddenConfigurations,
            GetOverriddenConfiguration: GetOverriddenConfiguration,
            UpdateOverriddenConfiguration: UpdateOverriddenConfiguration,
            AddOverriddenConfiguration: AddOverriddenConfiguration,
            GetOverriddenConfigurationHistoryDetailbyHistoryId: GetOverriddenConfigurationHistoryDetailbyHistoryId,
            GetOverriddenConfigSettingConfigs: GetOverriddenConfigSettingConfigs
           
        });
    }

    appControllers.service('VRCommon_OverriddenConfigAPIService', overriddenConfigAPIService);

})(appControllers);