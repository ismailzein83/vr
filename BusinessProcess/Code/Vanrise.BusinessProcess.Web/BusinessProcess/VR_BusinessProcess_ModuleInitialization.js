app.run(['BusinessProcess_BusinessProcessLogService', 'BusinessProcess_BPDefinitionService', 'BusinessProcess_VRWorkflowService',
    function (BusinessProcess_BusinessProcessLogService, BusinessProcess_BPDefinitionService, BusinessProcess_VRWorkflowService) {
    BusinessProcess_BusinessProcessLogService.registerLogToMaster();
    BusinessProcess_BPDefinitionService.registerObjectTrackingDrillDownToBPDefinition();
    BusinessProcess_VRWorkflowService.registerObjectTrackingDrillDownToVRWorkflow();
}]);