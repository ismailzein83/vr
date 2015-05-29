app.service('StrategyAPIService', function (BaseAPIService) {

    return ({
        GetAllStrategies: GetAllStrategies,
        GetStrategy: GetStrategy
    });

    function GetAllStrategies() {
        return BaseAPIService.get("/api/Strategy/GetAllStrategies",
            {
               
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