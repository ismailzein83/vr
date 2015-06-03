app.service('StrategyAPIService', function (BaseAPIService) {

    return ({
        GetFilteredStrategies: GetFilteredStrategies,
        AddStrategy: AddStrategy,
        UpdateStrategy: UpdateStrategy,
        GetStrategy: GetStrategy
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




});