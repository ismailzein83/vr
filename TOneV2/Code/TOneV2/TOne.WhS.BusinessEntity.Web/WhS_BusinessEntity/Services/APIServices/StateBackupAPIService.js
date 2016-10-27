(function (appControllers) {

    "use strict";
    StateBackupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];
    function StateBackupAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "StateBackup";

        function GetFilteredStateBackups(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredStateBackups"), input);
        }

        function RestoreData(stateBackupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "RestoreData"), {
                stateBackupId : stateBackupId});
        }

        return ({
            GetFilteredStateBackups: GetFilteredStateBackups,
            RestoreData: RestoreData
        });
    }

    appControllers.service('WhS_BE_StateBackupAPIService', StateBackupAPIService);
})(appControllers);