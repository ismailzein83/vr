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


    function GetStrategy(strategyId) {
        return BaseAPIService.get("/api/Strategy/GetStrategy",
            {
                StrategyId: strategyId
            }
           );
    }


    function AddStrategy(strategy) {
        return BaseAPIService.get("/api/Strategy/AddStrategy",
            {
                Strategy: strategy
            }
           );
    }


    function UpdateStrategy(strategy) {
        return BaseAPIService.get("/api/Strategy/UpdateStrategy",
            {
                Strategy: strategy
            }
           );
    }






});