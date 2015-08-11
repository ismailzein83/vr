
app.service('BlockedAttemptsAPIService', function (BaseAPIService) {

    return ({
        GetBlockedAttempts: GetBlockedAttempts,
      
    });

    function GetBlockedAttempts(blockedAttemptsInput) {
        return BaseAPIService.post("/api/BlockedAttempts/GetBlockedAttempts", blockedAttemptsInput);
    }

  

});