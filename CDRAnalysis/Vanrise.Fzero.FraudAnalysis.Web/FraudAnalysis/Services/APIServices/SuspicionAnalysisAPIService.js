app.service('SuspicionAnalysisAPIService', function (BaseAPIService) {

    return ({
        GetFilteredSuspiciousNumbers: GetFilteredSuspiciousNumbers,
        GetFilteredAccountSuspicionSummaries: GetFilteredAccountSuspicionSummaries,
        GetFilteredAccountSuspicionDetails: GetFilteredAccountSuspicionDetails,
        UpdateAccountCase: UpdateAccountCase,
        GetFraudResult: GetFraudResult,
        GetOperatorType: GetOperatorType
    });

    function GetFilteredSuspiciousNumbers(input) {
        return BaseAPIService.post("/api/SuspicionAnalysis/GetFilteredSuspiciousNumbers", input);
    }

    function GetFilteredAccountSuspicionSummaries(input) {
        return BaseAPIService.post("/api/SuspicionAnalysis/GetFilteredAccountSuspicionSummaries", input);
    }

    function GetFilteredAccountSuspicionDetails(input) {
        return BaseAPIService.post("/api/SuspicionAnalysis/GetFilteredAccountSuspicionDetails", input);
    }

    function UpdateAccountCase(input) {
        return BaseAPIService.post("/api/SuspicionAnalysis/UpdateAccountCase", input);
    }

    function GetFraudResult(fromDate, toDate, strategiesList, suspicionLevelsList, accountNumber) {
        return BaseAPIService.get("/api/SuspicionAnalysis/GetFraudResult", {
            fromDate: fromDate,
            toDate: toDate,
            strategiesList: strategiesList,
            suspicionLevelsList: suspicionLevelsList,
            accountNumber: accountNumber
        });
    }

    function GetOperatorType() {
        return BaseAPIService.get("/api/SuspicionAnalysis/GetOperatorType");
    }
});