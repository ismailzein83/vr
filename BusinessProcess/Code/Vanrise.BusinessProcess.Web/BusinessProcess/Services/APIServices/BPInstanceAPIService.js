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
        function HasRunningInstances(definitionId, entityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "HasRunningInstances"), {
                definitionId:definitionId,
                entityId: entityId
            });
        }
        function CreateNewProcess(createProcessInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPInstance", "CreateNewProcess"), createProcessInput);
        }

        return ({
            GetUpdated: GetUpdated,
            GetBeforeId: GetBeforeId,
            GetFilteredBPInstances: GetFilteredBPInstances,
            HasViewFilteredBPInstancesPermission:HasViewFilteredBPInstancesPermission,
            GetBPInstance: GetBPInstance,
            CreateNewProcess: CreateNewProcess,
            HasRunningInstances: HasRunningInstances
        });
    }

    appControllers.service('BusinessProcess_BPInstanceAPIService', BusinessProcess_BPInstanceAPIService);

})(appControllers);