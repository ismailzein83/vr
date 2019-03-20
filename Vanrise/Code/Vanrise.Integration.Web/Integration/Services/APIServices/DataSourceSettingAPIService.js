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

        function GetFileDataSourceDefinitionInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, controllerName, "GetFileDataSourceDefinitionInfo"), {
                filter: filter
            });
        }

        function AddFileDataSourceDefinition(fileDataSourceDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, controllerName, "AddFileDataSourceDefinition"), fileDataSourceDefinition);
        }

        function GetFileDataSourceDefinition(fileDataSourceDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, controllerName, "GetFileDataSourceDefinition"), {
                fileDataSourceDefinitionId: fileDataSourceDefinitionId
            });
        }

        function IsFileDataSourceDefinitionInUse(fileDataSourceDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Integration_ModuleConfig.moduleName, controllerName, "IsFileDataSourceDefinitionInUse"), {
                fileDataSourceDefinitionId: fileDataSourceDefinitionId
            });
        }

        return {

            GetFileDelayCheckerSettingsConfigs: GetFileDelayCheckerSettingsConfigs,
            GetFileMissingCheckerSettingsConfigs: GetFileMissingCheckerSettingsConfigs,
            GetFileDataSourceDefinitionInfo: GetFileDataSourceDefinitionInfo,
            AddFileDataSourceDefinition: AddFileDataSourceDefinition,
            GetFileDataSourceDefinition: GetFileDataSourceDefinition,
            IsFileDataSourceDefinitionInUse: IsFileDataSourceDefinitionInUse
        };
    }

    appControllers.service('VR_Integration_DataSourceSettingAPIService', dataSourceSettingAPIService);
})(appControllers);