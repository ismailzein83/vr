app.service('StrategyAPIService', function (BaseAPIService) {

    return ({
        GetAllStrategies: GetAllStrategies,
        GetFilteredStrategies: GetFilteredStrategies,
        AddStrategy: AddStrategy,
        UpdateStrategy: UpdateStrategy,
        GetStrategy: GetStrategy,
        GetFilters: GetFilters,
        GetAggregates: GetAggregates,
        GetPeriods: GetPeriods
    });


    function GetFilteredStrategies(fromRow, toRow, name, description) {
        return BaseAPIService.get("/api/Strategy/GetFilteredStrategies",
            {
                fromRow: fromRow,
                toRow: toRow,
                name: name,
                description: description
            }
           );
    }

    function GetAllStrategies() {
        return BaseAPIService.get("/api/Strategy/GetAllStrategies");
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