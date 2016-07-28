(function (appControllers) {

    'use strict';

    ExecutionFlowDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_Queueing_ModuleConfig'];

    function ExecutionFlowDefinitionAPIService(BaseAPIService, UtilsService, SecurityService, VR_Queueing_ModuleConfig) {
        return ({
            GetFilteredExecutionFlowDefinitions: GetFilteredExecutionFlowDefinitions,
            GetExecutionFlowDefinition: GetExecutionFlowDefinition,
            GetExecutionFlowStagesInfo :GetExecutionFlowStagesInfo ,
            AddExecutionFlowDefinition: AddExecutionFlowDefinition,
            HasAddExecutionFlowDefinition:HasAddExecutionFlowDefinition,
            UpdateExecutionFlowDefinition: UpdateExecutionFlowDefinition,
            HasUpdateExecutionFlowDefinition: HasUpdateExecutionFlowDefinition,
            GetExecutionFlowDefinitions: GetExecutionFlowDefinitions,
            GetQueueActivatorsConfig: GetQueueActivatorsConfig
        });

        function GetFilteredExecutionFlowDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'GetFilteredExecutionFlowDefinitions'), input);
        }
        function GetExecutionFlowDefinition(executionFlowDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'GetExecutionFlowDefinition'), {
                executionFlowDefinitionId: executionFlowDefinitionId
            });
        }
        function GetExecutionFlowStagesInfo(executionFlowDefinitionId, filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'GetExecutionFlowStagesInfo'), {
                executionFlowDefinitionId: executionFlowDefinitionId,
                filter: filter
            });
        }
        function AddExecutionFlowDefinition(executionFlowDefinitionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'AddExecutionFlowDefinition'), executionFlowDefinitionObject);
        }
        function HasAddExecutionFlowDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Queueing_ModuleConfig.moduleName, "ExecutionFlowDefinition", ['AddExecutionFlowDefinition']));
        }
        function UpdateExecutionFlowDefinition(executionFlowDefinitionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'UpdateExecutionFlowDefinition'), executionFlowDefinitionObject);
        }
        function HasUpdateExecutionFlowDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Queueing_ModuleConfig.moduleName, "ExecutionFlowDefinition", ['UpdateExecutionFlowDefinition']));
        }
        function GetExecutionFlowDefinitions(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'GetExecutionFlowDefinitions'), {
                filter: filter
            });
        }
        function GetQueueActivatorsConfig() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'GetQueueActivatorsConfig'));
        }
    }

    appControllers.service('VR_Queueing_ExecutionFlowDefinitionAPIService', ExecutionFlowDefinitionAPIService);

})(appControllers);
