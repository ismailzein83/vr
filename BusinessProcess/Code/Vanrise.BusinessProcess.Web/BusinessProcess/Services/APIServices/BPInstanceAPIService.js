(function (appControllers) {

    "use strict";

    BusinessProcess_BPInstanceAPIService.$inject = ['BaseAPIService', 'UtilsService','SecurityService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPInstanceAPIService(BaseAPIService, UtilsService, SecurityService, BusinessProcess_BP_ModuleConfig) {

        var controllerName = "BPInstance";

        function GetUpdated(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetUpdated"), input);
        }

        function GetBeforeId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBeforeId"), input);
        }

        function GetFilteredBPInstances(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetFilteredBPInstances"), input);
        }

        function HasViewFilteredBPInstancesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, ['GetFilteredBPInstances']));
        }

        function GetBPInstance(id) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPInstance"), {
                id: id
            });
        }

        function HasRunningInstances(hasRunningInstancesInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "HasRunningInstances"), hasRunningInstancesInput);
        }

        function CreateNewProcess(createProcessInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "CreateNewProcess"), createProcessInput);
        }

        function GetBPDefinitionSummary() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPDefinitionSummary"));
        }

        function GetBPInstanceDefinitionDetail(bpDefinitionId, bpInstanceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPInstanceDefinitionDetail"), {
                bpDefinitionId: bpDefinitionId,
                bpInstanceId: bpInstanceId
            });
        }

        function CancelProcess(cancelProcessInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "CancelProcess"), cancelProcessInput);
        }

        function GetBPInstanceBeforeInsertHandlerConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPInstanceBeforeInsertHandlerConfigs"));
        }

        function GetBPInstanceAfterInsertHandlerConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetBPInstanceAfterInsertHandlerConfigs"));
        }

        return ({
            GetUpdated: GetUpdated,
            GetBeforeId: GetBeforeId,
            GetFilteredBPInstances: GetFilteredBPInstances,
            HasViewFilteredBPInstancesPermission:HasViewFilteredBPInstancesPermission,
            GetBPInstance: GetBPInstance,
            CreateNewProcess: CreateNewProcess,
            HasRunningInstances: HasRunningInstances,
            GetBPDefinitionSummary: GetBPDefinitionSummary,
            CancelProcess: CancelProcess,
            GetBPInstanceDefinitionDetail: GetBPInstanceDefinitionDetail,
            GetBPInstanceBeforeInsertHandlerConfigs: GetBPInstanceBeforeInsertHandlerConfigs,
            GetBPInstanceAfterInsertHandlerConfigs: GetBPInstanceAfterInsertHandlerConfigs
        });
    }

    appControllers.service('BusinessProcess_BPInstanceAPIService', BusinessProcess_BPInstanceAPIService);
})(appControllers);