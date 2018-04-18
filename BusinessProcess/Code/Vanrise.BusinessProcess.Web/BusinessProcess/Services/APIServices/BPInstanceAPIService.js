(function (appControllers) {

    "use strict";
    BusinessProcess_BPInstanceAPIService.$inject = ['BaseAPIService', 'UtilsService','SecurityService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPInstanceAPIService(BaseAPIService, UtilsService, SecurityService, BusinessProcess_BP_ModuleConfig) {

        function GetUpdated(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "GetUpdated"), input);
        }

        function GetBeforeId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "GetBeforeId"), input);
        }

        function GetFilteredBPInstances(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "GetFilteredBPInstances"), input);
        }
        function HasViewFilteredBPInstancesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", ['GetFilteredBPInstances']));
        }
        function GetBPInstance(id) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "GetBPInstance"), {
                id: id
            });
        }
        function HasRunningInstances(hasRunningInstancesInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "HasRunningInstances"), hasRunningInstancesInput);
        }
        function CreateNewProcess(createProcessInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "CreateNewProcess"), createProcessInput);
        }
        function GetBPDefinitionSummary() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "GetBPDefinitionSummary"));
        }

        function GetBPInstanceDefinitionDetail(bpDefinitionId, bpInstanceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "GetBPInstanceDefinitionDetail"), {
                bpDefinitionId: bpDefinitionId,
                bpInstanceId: bpInstanceId
            });
        }

        function CancelProcess(cancelProcessInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "CancelProcess"), cancelProcessInput);
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
            GetBPInstanceDefinitionDetail: GetBPInstanceDefinitionDetail
        });
    }

    appControllers.service('BusinessProcess_BPInstanceAPIService', BusinessProcess_BPInstanceAPIService);

})(appControllers);