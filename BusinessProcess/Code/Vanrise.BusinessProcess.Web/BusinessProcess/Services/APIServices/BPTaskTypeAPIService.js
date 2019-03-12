(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPTaskTypeAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {

        function GetBPTaskTypeByTaskId(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTaskType", "GetBPTaskTypeByTaskId"), {
                taskId: taskId
            });
        }

        function GetBaseBPTaskTypeSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTaskType", "GetBaseBPTaskTypeSettingsConfigs"));
        };

        function GetBPTaskType(taskTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTaskType", "GetBPTaskType"), {
                taskTypeId: taskTypeId
            });
        }

        function GetBPTaskTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPTaskType", "GetBPTaskTypesInfo"), {
                filter: filter
            });
        }

        return ({
            GetBPTaskTypeByTaskId: GetBPTaskTypeByTaskId,
            GetBaseBPTaskTypeSettingsConfigs: GetBaseBPTaskTypeSettingsConfigs,
            GetBPTaskType: GetBPTaskType,
            GetBPTaskTypesInfo: GetBPTaskTypesInfo,
        });
    }

    appControllers.service('BusinessProcess_BPTaskTypeAPIService', BusinessProcess_BPTaskTypeAPIService);

})(appControllers);
