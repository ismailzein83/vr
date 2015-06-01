app.service('StrategyAPIService', function (BaseAPIService) {

    return ({
        GetFilteredStrategies: GetFilteredStrategies,
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


});