(function (appControllers) {

    'use strict';

    StrategyExecutionItemAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function StrategyExecutionItemAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {

        var controllerName = 'StrategyExecutionItem';

        function GetFilteredDetailsByCaseID(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetFilteredDetailsByCaseID'), input);
        }

        return {
            GetFilteredDetailsByCaseID: GetFilteredDetailsByCaseID
        };
    }

    appControllers.service('CDRAnalysis_FA_StrategyExecutionItemAPIService', StrategyExecutionItemAPIService);

})(appControllers);