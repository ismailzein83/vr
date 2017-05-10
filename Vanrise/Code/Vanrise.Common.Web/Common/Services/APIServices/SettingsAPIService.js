(function (appControllers) {

    "use strict";
    settingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function settingsAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controller = 'Settings';

        function GetFilteredSettings(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetFilteredSettings"), input);
        }

        function UpdateSetting(setting) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "UpdateSetting"), setting);
        }

        function GetSetting(settingId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetSetting"), {
                settingId: settingId
            });
        }

        function GetDistinctSettingCategories() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetDistinctSettingCategories"));
        }

        function HasUpdateSettingsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controller, ['UpdateSetting']));
        }
        function HasUpdateTechnicalSettingsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controller, ['UpdateTechnicalSetting']));
        }
        
        function GetSettingHistoryDetailbyHistoryId(settingHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, 'GetSettingHistoryDetailbyHistoryId'), {
                settingHistoryId: settingHistoryId
            });
        }
        function GetSettingsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, 'GetSettingsInfo'), {
                filter: filter
            });
        }
        return ({
            GetFilteredSettings: GetFilteredSettings,
            UpdateSetting: UpdateSetting,
            GetSetting: GetSetting,
            GetDistinctSettingCategories: GetDistinctSettingCategories,
            HasUpdateSettingsPermission: HasUpdateSettingsPermission,
            HasUpdateTechnicalSettingsPermission: HasUpdateTechnicalSettingsPermission,
            GetSettingHistoryDetailbyHistoryId: GetSettingHistoryDetailbyHistoryId,
            GetSettingsInfo: GetSettingsInfo
        });
    }

    appControllers.service('VRCommon_SettingsAPIService', settingsAPIService);

})(appControllers);