app.run(['BusinessProcess_BusinessProcessLogService', 'BusinessProcess_BPDefinitionService', function (BusinessProcess_BusinessProcessLogService, BusinessProcess_BPDefinitionService) {
    BusinessProcess_BusinessProcessLogService.registerLogToMaster();
    BusinessProcess_BPDefinitionService.registerObjectTrackingDrillDownToBPDefinition();
}]);