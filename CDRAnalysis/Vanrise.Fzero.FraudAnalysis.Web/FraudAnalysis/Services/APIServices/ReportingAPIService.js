app.service('ReportingAPIService', function (BaseAPIService) {

    return ({
        GetFilteredCasesProductivity: GetFilteredCasesProductivity,
        GetFilteredBlockedLines: GetFilteredBlockedLines
    });


    function GetFilteredCasesProductivity(input) {
        return BaseAPIService.post("/api/Reporting/GetFilteredCasesProductivity", input
           );
    }

    function GetFilteredBlockedLines(input) {
        return BaseAPIService.post("/api/Reporting/GetFilteredBlockedLines", input
           );
    }
    


});