app.service('CaseManagementAPIService', function (BaseAPIService) {

    return ({
        GetFilteredAccountCases: GetFilteredAccountCases,
        GetOperatorType: GetOperatorType,
        GetFilteredAccountSuspicionSummaries: GetFilteredAccountSuspicionSummaries,
        GetFilteredAccountSuspicionDetails: GetFilteredAccountSuspicionDetails,
        GetFilteredCasesByAccountNumber: GetFilteredCasesByAccountNumber,
        GetFilteredDetailsByCaseID: GetFilteredDetailsByCaseID,
        GetRelatedNumbersByAccountNumber: GetRelatedNumbersByAccountNumber,
        GetAccountCase: GetAccountCase,
        GetFilteredAccountCaseHistoryByCaseID: GetFilteredAccountCaseHistoryByCaseID,
        UpdateAccountCase: UpdateAccountCase,
        CancelAccountCases: CancelAccountCases,
        GetFilteredCasesByFilters: GetFilteredCasesByFilters,
        CancelSelectedAccountCases: CancelSelectedAccountCases
    });



    function CancelSelectedAccountCases(caseIds) {
        return BaseAPIService.post("/api/CaseManagement/CancelSelectedAccountCases", caseIds);
    }

    function CancelAccountCases(input) {
        return BaseAPIService.post("/api/CaseManagement/CancelAccountCases", input);
    }

    function GetFilteredAccountCases(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountCases", input);
    }

    function GetOperatorType() {
        return BaseAPIService.get("/api/CaseManagement/GetOperatorType");
    }


    function GetFilteredAccountSuspicionSummaries(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountSuspicionSummaries", input);
    }

    function GetFilteredAccountSuspicionDetails(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountSuspicionDetails", input);
    }

    function GetFilteredCasesByAccountNumber(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredCasesByAccountNumber", input);
    }

    function GetFilteredCasesByFilters(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredCasesByFilters", input);
    }

    function GetFilteredDetailsByCaseID(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredDetailsByCaseID", input);
    }

    function GetRelatedNumbersByAccountNumber(accountNumber) {
        return BaseAPIService.get("/api/CaseManagement/GetRelatedNumbersByAccountNumber", {
            accountNumber: accountNumber
        });
    }

    function GetAccountCase(caseID) {
        return BaseAPIService.get("/api/CaseManagement/GetAccountCase", {
            caseID: caseID
        });
    }

    function GetFilteredAccountCaseHistoryByCaseID(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountCaseHistoryByCaseID", input);
    }

    function UpdateAccountCase(input) {
        return BaseAPIService.post("/api/CaseManagement/UpdateAccountCase", input);
    }
});
