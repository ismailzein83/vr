(function (appControllers) {

    "use strict";

    dataSourceSettingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Integration_ModuleConfig'];

    function dataSourceSettingAPIService(BaseAPIService, UtilsService, VR_Integration_ModuleConfig) {
        var controllerName = 'DataSourceSetting';

        function GetFileDelayCheckerSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, controllerName, "GetFileDelayCheckerSettingsConfigs"));
        }

        function GetFileMissingCheckerSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, controllerName, "GetFileMissingCheckerSettingsConfigs"));
        }

        return {
            GetFileDelayCheckerSettingsConfigs: GetFileDelayCheckerSettingsConfigs,
            GetFileMissingCheckerSettingsConfigs: GetFileMissingCheckerSettingsConfigs
        };
    }

    appControllers.service('VR_Integration_DataSourceSettingAPIService', dataSourceSettingAPIService);
})(appControllers);