(function (appControllers) {

    "use strict";
    StateBackupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService' ,'WhS_BE_ModuleConfig'];
    function StateBackupAPIService(BaseAPIService, UtilsService, SecurityService , WhS_BE_ModuleConfig) {

        var controllerName = "StateBackup";

        function GetFilteredStateBackups(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredStateBackups"), input);
        }

        function RestoreData(stateBackupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "RestoreData"), {
                stateBackupId : stateBackupId});
        }

        function GetStateBackupTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetStateBackupTypes"));
        }

        function HasRestoreDataPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['RestoreData']));
        }
        return ({
            GetFilteredStateBackups: GetFilteredStateBackups,
            RestoreData: RestoreData,
            HasRestoreDataPermission:HasRestoreDataPermission,
            GetStateBackupTypes: GetStateBackupTypes
        });
    }

    appControllers.service('WhS_BE_StateBackupAPIService', StateBackupAPIService);
})(appControllers);