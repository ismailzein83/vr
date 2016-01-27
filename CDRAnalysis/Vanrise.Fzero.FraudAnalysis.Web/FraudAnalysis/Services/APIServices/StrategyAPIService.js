(function (appControllers) {

    'use strict';

    StrategyAPIService.$inject = ['BaseAPIService'];

    function StrategyAPIService(BaseAPIService) {
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
            return BaseAPIService.post("/api/Strategy/GetFilteredStrategies", input
               );
        }

        function GetStrategies(periodId, isEnabled) {
            return BaseAPIService.get("/api/Strategy/GetStrategies",
                {
                    PeriodId: periodId,
                    IsEnabled: isEnabled
                });
        }

        function GetStrategy(StrategyId) {
            return BaseAPIService.get("/api/Strategy/GetStrategy",
                {
                    StrategyId: StrategyId
                }
               );
        }

        function GetFilters() {
            return BaseAPIService.get("/api/Strategy/GetFilters");
        }

        function GetAggregates() {
            return BaseAPIService.get("/api/Strategy/GetAggregates");
        }

        function AddStrategy(strategy) {
            return BaseAPIService.post("/api/Strategy/AddStrategy",
                strategy
               );
        }

        function UpdateStrategy(strategy) {
            return BaseAPIService.post("/api/Strategy/UpdateStrategy",
                strategy
               );
        }
    }

    appControllers.service('StrategyAPIService', StrategyAPIService);

})(appControllers);
