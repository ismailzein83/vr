app.run(['BusinessProcess_BusinessProcessLogService', 'BusinessProcess_BPDefinitionService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_ProcessSynchronisationService',
    function (BusinessProcess_BusinessProcessLogService, BusinessProcess_BPDefinitionService, BusinessProcess_VRWorkflowService, BusinessProcess_ProcessSynchronisationService) {
        BusinessProcess_BusinessProcessLogService.registerLogToMaster();
        BusinessProcess_BPDefinitionService.registerObjectTrackingDrillDownToBPDefinition();
        BusinessProcess_VRWorkflowService.registerObjectTrackingDrillDownToVRWorkflow();
        BusinessProcess_ProcessSynchronisationService.registerObjectTrackingDrillDownToProcessSynchronisation();
    }]);