(function (appControllers) {

    'use strict';

    ExecutionFlowDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function ExecutionFlowDefinitionAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetExecutionFlowDefinitions: GetExecutionFlowDefinitions
        });

        function GetExecutionFlowDefinitions(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlowDefinition', 'GetExecutionFlowDefinitions'), {
                filter: filter
            });
        }
    }

    appControllers.service('VR_Queueing_ExecutionFlowDefinitionAPIService', ExecutionFlowDefinitionAPIService);

})(appControllers);
