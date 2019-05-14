(function (appControllers) {

    'use strict';

    RDBDataRecordStorageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function RDBDataRecordStorageAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {

        var controllerName = 'RDBDataRecordStorage';

        function GetRDBDataRecordStorageJoinSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetRDBDataRecordStorageJoinSettingsConfigs"));
        }

        function GetRDBDataRecordStorageExpressionFieldSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetRDBDataRecordStorageExpressionFieldSettingsConfigs"));
        }

        function GetRDBDataRecordStorageSettingsFilterConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetRDBDataRecordStorageSettingsFilterConfigs"));
        }

        return {
            GetRDBDataRecordStorageJoinSettingsConfigs: GetRDBDataRecordStorageJoinSettingsConfigs,
            GetRDBDataRecordStorageExpressionFieldSettingsConfigs: GetRDBDataRecordStorageExpressionFieldSettingsConfigs,
            GetRDBDataRecordStorageSettingsFilterConfigs: GetRDBDataRecordStorageSettingsFilterConfigs,
        };
    }
    appControllers.service('VR_GenericData_RDBDataRecordStorageAPIService', RDBDataRecordStorageAPIService);

})(appControllers);