(function (appControllers) {

    'use strict';

    ExecutionFlowAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function ExecutionFlowAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetExecutionFlowDefinitions: GetExecutionFlowDefinitions,
            GetFilteredExecutionFlows: GetFilteredExecutionFlows,
            GetExecutionFlow: GetExecutionFlow,
            UpdateExecutionFlow: UpdateExecutionFlow,
            AddExecutionFlow: AddExecutionFlow,
            GetExecutionFlows: GetExecutionFlows,
            GetExecutionFlowStatusSummary:GetExecutionFlowStatusSummary
        });

        function GetExecutionFlowDefinitions(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetExecutionFlowDefinitions'), {
                filter:filter
            });
        }

        function GetFilteredExecutionFlows(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetFilteredExecutionFlows'), input);
        }

        function GetExecutionFlow(executionFlowId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetExecutionFlow'), { executionFlowId: executionFlowId });
        }
        
        function UpdateExecutionFlow(executionFlowObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'UpdateExecutionFlow'), executionFlowObject);
        }

        function AddExecutionFlow(executionFlowObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'AddExecutionFlow'), executionFlowObject);
        }

        function GetExecutionFlows(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetExecutionFlows'), {
                filter:filter
            });
        }

        function GetExecutionFlowStatusSummary() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetExecutionFlowStatusSummary'));
        }
        
        
    }

    appControllers.service('VR_Queueing_ExecutionFlowAPIService', ExecutionFlowAPIService);

})(appControllers);
