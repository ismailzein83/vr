app.service('SuspicionAnalysisAPIService', function (BaseAPIService) {

    return ({
        GetFilteredSuspiciousNumbers: GetFilteredSuspiciousNumbers,
        GetFraudResult: GetFraudResult,
        GetOperatorType: GetOperatorType
    });


    function GetOperatorType() {
        return BaseAPIService.get("/api/SuspicionAnalysis/GetOperatorType");
    }

    function GetFilteredSuspiciousNumbers(input) {
        return BaseAPIService.post("/api/SuspicionAnalysis/GetFilteredSuspiciousNumbers", input);
    }


    function GetFraudResult(fromDate, toDate, strategiesList, suspicionLevelsList, accountNumber) {
        return BaseAPIService.get("/api/SuspicionAnalysis/GetFraudResult",
            {
                fromDate: fromDate,
                toDate: toDate,
                strategiesList: strategiesList,
                suspicionLevelsList: suspicionLevelsList,
                accountNumber: accountNumber
            }
           );
    }


});