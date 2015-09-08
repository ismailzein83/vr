app.service('CaseManagementAPIService', function (BaseAPIService) {

    return ({
        GetFilteredAccountCases: GetFilteredAccountCases,
        GetFilteredSuspiciousNumbers: GetFilteredSuspiciousNumbers,
        GetFraudResult: GetFraudResult,
        GetOperatorType: GetOperatorType,
        SaveAccountCase: SaveAccountCase,

        GetFilteredAccountSuspicionSummaries: GetFilteredAccountSuspicionSummaries,
        GetFilteredAccountSuspicionDetails: GetFilteredAccountSuspicionDetails,
        GetFilteredNormalCDRs: GetFilteredNormalCDRs,
        UpdateAccountCase: UpdateAccountCase
    });

    function GetFilteredAccountCases(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountCases", input);
    }

    function GetFilteredSuspiciousNumbers(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredSuspiciousNumbers", input);
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

    function SaveAccountCase(accountCase) {
        return BaseAPIService.post("/api/CaseManagement/SaveAccountCase", accountCase);
    }

    /* *** New Functions *** */

    function GetFilteredAccountSuspicionSummaries(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountSuspicionSummaries", input);
    }

    function GetFilteredAccountSuspicionDetails(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountSuspicionDetails", input);
    }

    function GetFilteredNormalCDRs(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredNormalCDRs", input);
    }

    function UpdateAccountCase(input) {
        return BaseAPIService.post("/api/CaseManagement/UpdateAccountCase", input);
    }
});
