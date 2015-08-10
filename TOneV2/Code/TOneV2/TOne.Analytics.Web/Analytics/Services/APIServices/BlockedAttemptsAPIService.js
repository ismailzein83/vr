
app.service('BlockedAttemptsAPIService', function (BaseAPIService) {

    return ({
        GetBlockedAttempts: GetBlockedAttempts,
      
    });

    function GetBlockedAttempts(getTrafficStatisticSummaryInput) {
        return BaseAPIService.post("/api/BlockedAttempts/GetBlockedAttempts", getTrafficStatisticSummaryInput);
    }

  

});