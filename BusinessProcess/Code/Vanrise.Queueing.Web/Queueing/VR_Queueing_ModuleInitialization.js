app.run(['VR_Queueing_ExecutionFlowService', 'VR_Queueing_QueueInstanceService', 'VR_Queueing_ExecutionFlowDefinitionService', function (VR_Queueing_ExecutionFlowService,VR_Queueing_QueueInstanceService, VR_Queueing_ExecutionFlowDefinitionService) {
    VR_Queueing_QueueInstanceService.registerDrillDownToExecutionFlow();
    VR_Queueing_ExecutionFlowDefinitionService.registerObjectTrackingDrillDownToQueueExecutionFlowDefinition();
    VR_Queueing_ExecutionFlowService.registerObjectTrackingDrillDownToQueueExecutionFlow();
}]);