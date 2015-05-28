app.service('StrategyAPIService', function (BaseAPIService) {

    return ({
        GetAllStrategies: GetAllStrategies,
        GetStrategy: GetStrategy
    });

    function GetAllStrategies() {
        return BaseAPIService.get("/api/User/GetAllStrategies",
            {
               
            }
           );
    }

    function GetStrategy(strategyId) {
        return BaseAPIService.get("/api/User/GetStrategy",
            {
                StrategyId: strategyId
            }
           );
    }


});