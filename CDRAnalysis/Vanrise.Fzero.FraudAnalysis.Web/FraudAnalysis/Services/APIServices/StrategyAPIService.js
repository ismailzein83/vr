app.service('StrategyAPIService', function (BaseAPIService) {

    return ({
        GetStrategies: GetStrategies,
        GetFilteredStrategies: GetFilteredStrategies,
        AddStrategy: AddStrategy,
        UpdateStrategy: UpdateStrategy,
        GetStrategy: GetStrategy,
        GetFilters: GetFilters,
        GetAggregates: GetAggregates,
        GetPeriods: GetPeriods
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


    function GetFilters() {
        return BaseAPIService.get("/api/Strategy/GetFilters");
    }

    function GetAggregates() {
        return BaseAPIService.get("/api/Strategy/GetAggregates");
    }

    function GetPeriods() {
        return BaseAPIService.get("/api/Strategy/GetPeriods");
    }




});