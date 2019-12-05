﻿app.run(['BusinessProcess_BusinessProcessLogService', 'BusinessProcess_BPDefinitionService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_ProcessSynchronisationService', 'BusinessProcess_BPInstanceService', 'BusinessProcess_TaskTypeActionService', 'BusinessProcess_BPTaskService',
    function (BusinessProcess_BusinessProcessLogService, BusinessProcess_BPDefinitionService, BusinessProcess_VRWorkflowService, BusinessProcess_ProcessSynchronisationService, BusinessProcess_BPInstanceService, BusinessProcess_TaskTypeActionService, BusinessProcess_BPTaskService) {

        BusinessProcess_BusinessProcessLogService.registerLogToMaster();
        BusinessProcess_BPDefinitionService.registerObjectTrackingDrillDownToBPDefinition();
        BusinessProcess_VRWorkflowService.registerObjectTrackingDrillDownToVRWorkflow();
        BusinessProcess_ProcessSynchronisationService.registerObjectTrackingDrillDownToProcessSynchronisation();
        BusinessProcess_BPInstanceService.registerDrillDownToSchdeulerTask();
        BusinessProcess_BPInstanceService.registerOpenBPInstanceViewerAction();
        BusinessProcess_TaskTypeActionService.registerExecuteAction();
        BusinessProcess_BPInstanceService.registerStartBPProcessAction();
        BusinessProcess_BPInstanceService.registerStartBPProcessCustomAction();
        BusinessProcess_TaskTypeActionService.registerOpenRDLCReportAction();
        BusinessProcess_BPTaskService.registerOpenBPTaskBEAction();
    }]);