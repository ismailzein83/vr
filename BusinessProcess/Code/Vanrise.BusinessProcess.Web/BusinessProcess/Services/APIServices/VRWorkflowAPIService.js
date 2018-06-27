(function (appControllers) {

    "use strict";
    BusinessProcess_VRWorkflowAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_VRWorkflowAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {
        var controllerName = 'VRWorkflow';

        function GetFilteredVRWorkflows(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetFilteredVRWorkflows"), input);
        }

        function GetVRWorkflow(vrWorkflowId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflow"), { vrWorkflowId: vrWorkflowId });
        }

        function InsertVRWorkflow(vrWorkflow) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "InsertVRWorkflow"), vrWorkflow);
        }

        function UpdateVRWorkflow(vrWorkflow) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "UpdateVRWorkflow"), vrWorkflow);
        }

        return ({
            GetFilteredVRWorkflows: GetFilteredVRWorkflows,
            GetVRWorkflow: GetVRWorkflow,
            InsertVRWorkflow: InsertVRWorkflow,
            UpdateVRWorkflow: UpdateVRWorkflow
        });
    }

    appControllers.service('BusinessProcess_VRWorkflowAPIService', BusinessProcess_VRWorkflowAPIService);

})(appControllers);