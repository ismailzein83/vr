(function (appControllers) {

    'use strict';

    ExecutionFlowAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_Queueing_ModuleConfig'];

    function ExecutionFlowAPIService(BaseAPIService, UtilsService, SecurityService, VR_Queueing_ModuleConfig) {
        return ({
            GetExecutionFlowDefinitions: GetExecutionFlowDefinitions,
            GetFilteredExecutionFlows: GetFilteredExecutionFlows,
            AddExecutionFlow: AddExecutionFlow,
            HasAddExecutionFlow: HasAddExecutionFlow,
            GetExecutionFlow: GetExecutionFlow,
            UpdateExecutionFlow: UpdateExecutionFlow,
            HasUpdateExecutionFlow:HasUpdateExecutionFlow,
            GetExecutionFlows: GetExecutionFlows
          
        });

        function GetExecutionFlowDefinitions(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetExecutionFlowDefinitions'), {
                filter:filter
            });
        }
        function GetFilteredExecutionFlows(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetFilteredExecutionFlows'), input);
        }
        function AddExecutionFlow(executionFlowObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'AddExecutionFlow'), executionFlowObject);
        }
        function HasAddExecutionFlow() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Queueing_ModuleConfig.moduleName, "ExecutionFlow", ['AddExecutionFlow']));
        }
        function UpdateExecutionFlow(executionFlowObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'UpdateExecutionFlow'), executionFlowObject);
        }
        function HasUpdateExecutionFlow() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Queueing_ModuleConfig.moduleName, "ExecutionFlow", ['UpdateExecutionFlow']));
        }

        function GetExecutionFlow(executionFlowId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetExecutionFlow'), { executionFlowId: executionFlowId });
        }
        function GetExecutionFlows(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetExecutionFlows'), {
                filter:filter
            });
        }

       
        
        
    }

    appControllers.service('VR_Queueing_ExecutionFlowAPIService', ExecutionFlowAPIService);

})(appControllers);
