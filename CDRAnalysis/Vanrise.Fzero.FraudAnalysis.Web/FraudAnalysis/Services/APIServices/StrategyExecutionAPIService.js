(function (appControllers) {

    'use strict';

    StrategyExecutionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function StrategyExecutionAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {

        var controllerName = 'StrategyExecution';

        function GetFilteredStrategyExecutions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetFilteredStrategyExecutions'), input);
        }

        return {
            GetFilteredStrategyExecutions: GetFilteredStrategyExecutions
        };
    }

    appControllers.service('CDRAnalysis_FA_StrategyExecutionAPIService', StrategyExecutionAPIService);

})(appControllers);