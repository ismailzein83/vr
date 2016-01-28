(function (appControllers) {

    'use strict';

    StrategyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function StrategyAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {
        return ({
            GetFilteredStrategies: GetFilteredStrategies,
            GetStrategies: GetStrategies,
            GetStrategy: GetStrategy,
            GetFilters: GetFilters,
            GetAggregates: GetAggregates,
            AddStrategy: AddStrategy,
            UpdateStrategy: UpdateStrategy
        });

        function GetFilteredStrategies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'Strategy', 'GetFilteredStrategies'), input);
        }

        function GetStrategies(periodId, isEnabled) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'Strategy', 'GetStrategies'), {
                PeriodId: periodId,
                IsEnabled: isEnabled
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
