app.service('CaseManagementAPIService', function (BaseAPIService) {

    return ({
        GetFilteredAccountCases: GetFilteredAccountCases,
        GetOperatorType: GetOperatorType,
        GetFilteredAccountSuspicionSummaries: GetFilteredAccountSuspicionSummaries,
        GetFilteredAccountSuspicionDetails: GetFilteredAccountSuspicionDetails,
        GetFilteredCasesByAccountNumber: GetFilteredCasesByAccountNumber,
        GetFilteredDetailsByCaseID: GetFilteredDetailsByCaseID,
        UpdateAccountCase: UpdateAccountCase,
        CancelAccountCases: CancelAccountCases
    });


    function CancelAccountCases(input) {
        return BaseAPIService.post("/api/CaseManagement/CancelAccountCases", input);
    }

    function GetFilteredAccountCases(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountCases", input);
    }

    function GetOperatorType() {
        return BaseAPIService.get("/api/CaseManagement/GetOperatorType");
    }

    /* *** New Functions *** */

    function GetFilteredAccountSuspicionSummaries(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountSuspicionSummaries", input);
    }

    function GetFilteredAccountSuspicionDetails(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountSuspicionDetails", input);
    }

    function GetFilteredCasesByAccountNumber(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredCasesByAccountNumber", input);
    }

    function GetFilteredDetailsByCaseID(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredDetailsByCaseID", input);
    }

    function UpdateAccountCase(input) {
        return BaseAPIService.post("/api/CaseManagement/UpdateAccountCase", input);
    }
});
