app.service('CaseManagementAPIService', function (BaseAPIService) {

    return ({
        SaveAccountCase: SaveAccountCase,
        GetFilteredAccountCases: GetFilteredAccountCases,
        GetFilteredSuspiciousNumbers: GetFilteredSuspiciousNumbers,
        GetFilteredAccountSuspicionSummaries: GetFilteredAccountSuspicionSummaries,
        GetFilteredAccountSuspicionDetails: GetFilteredAccountSuspicionDetails,
        UpdateAccountCase: UpdateAccountCase,
        GetFraudResult: GetFraudResult,
        GetOperatorType: GetOperatorType
    });

    function SaveAccountCase(accountCase) {
        return BaseAPIService.post("/api/CaseManagement/SaveAccountCase",
            accountCase
           );
    }

    function GetFilteredAccountCases(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountCases", input);
    }

    function GetFilteredSuspiciousNumbers(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredSuspiciousNumbers", input);
    }

    function GetFilteredAccountSuspicionSummaries(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountSuspicionSummaries", input);
    }

    function GetFilteredAccountSuspicionDetails(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountSuspicionDetails", input);
    }

    function UpdateAccountCase(input) {
        return BaseAPIService.post("/api/CaseManagement/UpdateAccountCase", input);
    }

    function GetFraudResult(fromDate, toDate, strategiesList, suspicionLevelsList, accountNumber) {
        return BaseAPIService.get("/api/CaseManagement/GetFraudResult", {
            fromDate: fromDate,
            toDate: toDate,
            strategiesList: strategiesList,
            suspicionLevelsList: suspicionLevelsList,
            accountNumber: accountNumber
        });
    }

    function GetOperatorType() {
        return BaseAPIService.get("/api/CaseManagement/GetOperatorType");
    }

    


});