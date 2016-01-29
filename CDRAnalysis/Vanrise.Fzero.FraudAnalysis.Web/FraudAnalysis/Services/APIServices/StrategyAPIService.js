(function (appControllers) {

    'use strict';

    StrategyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function StrategyAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {
        return ({
            GetFilteredStrategies: GetFilteredStrategies,
            GetStrategiesInfo: GetStrategiesInfo,
            GetStrategy: GetStrategy,
            GetFilters: GetFilters,
            GetAggregates: GetAggregates,
            AddStrategy: AddStrategy,
            UpdateStrategy: UpdateStrategy
        });

        function GetFilteredStrategies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'Strategy', 'GetFilteredStrategies'), input);
        }

        function GetStrategiesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'Strategy', 'GetStrategiesInfo'), {
                filter: filter
            });
        }

        function GetStrategy(strategyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'Strategy', 'GetStrategy'), {
                strategyId: strategyId
            });
        }

        function GetFilters() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'Strategy', 'GetFilters'));
        }

        function GetAggregates() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'Strategy', 'GetAggregates'));
        }

        function AddStrategy(strategy) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'Strategy', 'AddStrategy'), strategy);
        }

        function UpdateStrategy(strategy) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'Strategy', 'UpdateStrategy'), strategy);
        }
    }

    appControllers.service('StrategyAPIService', StrategyAPIService);

})(appControllers);
