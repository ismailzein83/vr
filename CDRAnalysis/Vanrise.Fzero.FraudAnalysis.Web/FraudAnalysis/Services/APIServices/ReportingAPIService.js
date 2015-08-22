app.service('ReportingAPIService', function (BaseAPIService) {

    return ({
        GetFilteredCasesProductivity: GetFilteredCasesProductivity,
        GetFilteredBlockedLines: GetFilteredBlockedLines,
        GetFilteredLinesDetected: GetFilteredLinesDetected
    });


    function GetFilteredCasesProductivity(input) {
        return BaseAPIService.post("/api/Reporting/GetFilteredCasesProductivity", input
           );
    }

    function GetFilteredBlockedLines(input) {
        return BaseAPIService.post("/api/Reporting/GetFilteredBlockedLines", input
           );
    }


    function GetFilteredLinesDetected(input) {
        return BaseAPIService.post("/api/Reporting/GetFilteredLinesDetected", input
           );
    }
    


});