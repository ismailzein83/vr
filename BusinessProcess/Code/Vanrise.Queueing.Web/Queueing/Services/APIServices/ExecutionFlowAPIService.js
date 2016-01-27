(function (appControllers) {

    'use strict';

    ExecutionFlowAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function ExecutionFlowAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetExecutionFlowDefinitions: GetExecutionFlowDefinitions,
            GetFilteredExecutionFlows: GetFilteredExecutionFlows
        });

        function GetExecutionFlowDefinitions(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetFilteredExecutionFlowDefinitions'), {
                filter: filter
            });
        }

        function GetFilteredExecutionFlows(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionFlow', 'GetFilteredExecutionFlows'), {
                filter: filter
            });
        }

    }

    appControllers.service('VR_Queueing_ExecutionFlowAPIService', ExecutionFlowAPIService);

})(appControllers);
