app.service('CaseManagementAPIService', function (BaseAPIService) {

    return ({
        SaveAccountCase: SaveAccountCase
    });

    function SaveAccountCase(accountCase) {
        return BaseAPIService.post("/api/CaseManagement/SaveAccountCase",
            accountCase
           );
    }


});