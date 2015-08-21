app.service('ReportingAPIService', function (BaseAPIService) {

    return ({
        GetFilteredCasesProductivity: GetFilteredCasesProductivity
    });


    function GetFilteredCasesProductivity(input) {
        return BaseAPIService.post("/api/Reporting/GetFilteredCasesProductivity", input
           );
    }
    


});