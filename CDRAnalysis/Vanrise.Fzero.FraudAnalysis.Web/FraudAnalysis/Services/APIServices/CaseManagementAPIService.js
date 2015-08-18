app.service('CaseManagementAPIService', function (BaseAPIService) {

    return ({
        SaveAccountCase: SaveAccountCase,
        GetFilteredAccountCases: GetFilteredAccountCases
    });

    function SaveAccountCase(accountCase) {
        return BaseAPIService.post("/api/CaseManagement/SaveAccountCase",
            accountCase
           );
    }

    function GetFilteredAccountCases(input) {
        return BaseAPIService.post("/api/CaseManagement/GetFilteredAccountCases", input);
    }



    


});