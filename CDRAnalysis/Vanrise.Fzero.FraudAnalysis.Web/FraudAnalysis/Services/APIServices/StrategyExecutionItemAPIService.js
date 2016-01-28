(function (appControllers) {

    'use strict';

    StrategyExecutionItemAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function StrategyExecutionItemAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {
        return {
            GetFilteredDetailsByCaseID: GetFilteredDetailsByCaseID
        };

        function GetFilteredDetailsByCaseID(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'StrategyExecutionItem', 'GetFilteredDetailsByCaseID'), input);
        }
    }

    appControllers.service('CDRAnalysis_FA_StrategyExecutionItemAPIService', StrategyExecutionItemAPIService);

})(appControllers);