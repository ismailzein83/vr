(function (appControllers) {

    'use strict';

    ExecutionFlowDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function ExecutionFlowDefinitionAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetFilteredExecutionFlowDefinitions: GetFilteredExecutionFlowDefinitions,
            GetExecutionFlowDefinition: GetExecutionFlowDefinition,
            UpdateExecutionFlowDefinition: UpdateExecutionFlowDefinition,
            AddExecutionFlowDefinition: AddExecutionFlowDefinition,
            GetExecutionFlowDefinitions: GetExecutionFlowDefinitions
        });

        function GetFilteredExecutionFlowDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'GetFilteredExecutionFlowDefinitions'), input);
        }

        function GetExecutionFlowDefinition(executionFlowDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'GetExecutionFlowDefinition'), { executionFlowDefinitionId: executionFlowDefinitionId });
        }

        function UpdateExecutionFlowDefinition(executionFlowDefinitionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'UpdateExecutionFlowDefinition'), executionFlowDefinitionObject);
        }

        function AddExecutionFlowDefinition(executionFlowDefinitionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'AddExecutionFlowDefinition'), executionFlowDefinitionObject);
        }

        function GetExecutionFlowDefinitions(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'GetExecutionFlowDefinitions'), {
                filter: filter
            });
        }
    }

    appControllers.service('VR_Queueing_ExecutionFlowDefinitionAPIService', ExecutionFlowDefinitionAPIService);

})(appControllers);
