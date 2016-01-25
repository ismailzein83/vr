app.service('StrategyExecutionAPIService', function (BaseAPIService) {

    return ({
        GetFilteredStrategyExecutions: GetFilteredStrategyExecutions
    });


    function GetFilteredStrategyExecutions(input) {
        return BaseAPIService.post("/api/StrategyExecution/GetFilteredStrategyExecutions", input
           );
    }
});