(function (appControllers) {

    'use strict';

    ExecutionFlowAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function ExecutionFlowAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetExecutionFlowDefinitions: GetExecutionFlowDefinitions,
            GetFilteredExecutionFlows: GetFilteredExecutionFlows,
            GetExecutionFlow: GetExecutionFlow,
            UpdateExecutionFlow: UpdateExecutionFlow
        });

        function GetExecutionFlowDefinitions() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetFilteredExecutionFlowDefinitions'));
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
        
    }

    appControllers.service('VR_Queueing_ExecutionFlowAPIService', ExecutionFlowAPIService);

})(appControllers);
