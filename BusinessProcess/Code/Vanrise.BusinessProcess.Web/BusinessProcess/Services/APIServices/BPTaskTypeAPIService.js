(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPTaskTypeAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {

        var controllerName = "BPTaskType";

        function GetBPTaskTypeByTaskId(taskId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPTaskTypeByTaskId"), {
                taskId: taskId
            });
        }

        function GetBaseBPTaskTypeSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBaseBPTaskTypeSettingsConfigs"));
        }

        function GetBPTaskType(taskTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPTaskType"), {
                taskTypeId: taskTypeId
            });
        }

        function GetBPTaskTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPTaskTypesInfo"), {
                filter: filter
            });
        }

        function GetVRWorkflowTaskAssigneesSettingExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflowTaskAssigneesSettingExtensionConfigs"), {});
        }

        function GetBPGenericTaskTypeActionSettingsExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPGenericTaskTypeActionSettingsExtensionConfigs"), {});
        }

        function GetBPGenericTaskTypeActionFilterConditionExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPGenericTaskTypeActionFilterConditionExtensionConfigs"), {});
        }

        return ({
            GetBPTaskTypeByTaskId: GetBPTaskTypeByTaskId,
            GetBaseBPTaskTypeSettingsConfigs: GetBaseBPTaskTypeSettingsConfigs,
            GetBPTaskType: GetBPTaskType,
            GetBPTaskTypesInfo: GetBPTaskTypesInfo,
            GetVRWorkflowTaskAssigneesSettingExtensionConfigs: GetVRWorkflowTaskAssigneesSettingExtensionConfigs,
            GetBPGenericTaskTypeActionSettingsExtensionConfigs: GetBPGenericTaskTypeActionSettingsExtensionConfigs,
            GetBPGenericTaskTypeActionFilterConditionExtensionConfigs: GetBPGenericTaskTypeActionFilterConditionExtensionConfigs
        });
    }

    appControllers.service('BusinessProcess_BPTaskTypeAPIService', BusinessProcess_BPTaskTypeAPIService);

})(appControllers);
