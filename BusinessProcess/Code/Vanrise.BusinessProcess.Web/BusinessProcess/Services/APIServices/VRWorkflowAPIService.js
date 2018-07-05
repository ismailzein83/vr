(function (appControllers) {

    "use strict";

    BusinessProcess_VRWorkflowAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig', 'SecurityService'];

    function BusinessProcess_VRWorkflowAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig, SecurityService) {
        var controllerName = 'VRWorkflow';

        function GetFilteredVRWorkflows(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetFilteredVRWorkflows"), input);
        }

        function GetVRWorkflowEditorRuntime(vrWorkflowId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflowEditorRuntime"), { vrWorkflowId: vrWorkflowId });
        }

        function GetVRWorkflowVariableTypeExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflowVariableTypeExtensionConfigs"), {});
        }

        function GetVRWorkflowArgumentTypeDescription(vrWorkflowArgumentType) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflowArgumentTypeDescription"), vrWorkflowArgumentType);
        }

        function InsertVRWorkflow(vrWorkflow) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "InsertVRWorkflow"), vrWorkflow);
        }

        function UpdateVRWorkflow(vrWorkflow) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "UpdateVRWorkflow"), vrWorkflow);
        }

        function HasAddVRWorkflowPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, ['InsertVRWorkflow']));
        }

        function HasEditVRWorkflowPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, ['UpdateVRWorkflow']));
        }

        return ({
            GetFilteredVRWorkflows: GetFilteredVRWorkflows,
            GetVRWorkflowEditorRuntime: GetVRWorkflowEditorRuntime,
            GetVRWorkflowVariableTypeExtensionConfigs: GetVRWorkflowVariableTypeExtensionConfigs,
            GetVRWorkflowArgumentTypeDescription: GetVRWorkflowArgumentTypeDescription,
            InsertVRWorkflow: InsertVRWorkflow,
            UpdateVRWorkflow: UpdateVRWorkflow,
            HasAddVRWorkflowPermission: HasAddVRWorkflowPermission,
            HasEditVRWorkflowPermission: HasEditVRWorkflowPermission
        });
    }

    appControllers.service('BusinessProcess_VRWorkflowAPIService', BusinessProcess_VRWorkflowAPIService);

})(appControllers);