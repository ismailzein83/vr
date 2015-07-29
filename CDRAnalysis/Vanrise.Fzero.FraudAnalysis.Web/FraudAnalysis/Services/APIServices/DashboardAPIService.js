app.service('DashboardAPIService', function (BaseAPIService) {

    return ({
        GetCasesSummary: GetCasesSummary,
        GetStrategyCases: GetStrategyCases,
        GetBTSCases: GetBTSCases,
        GetCellCases: GetCellCases
    });


    function GetCasesSummary(fromDate, toDate) {
        return BaseAPIService.get("/api/Strategy/GetCasesSummary",
            {
                fromDate: fromDate,
                toDate: toDate
            }
           );
    }

    function GetStrategyCases(fromDate, toDate) {
        return BaseAPIService.get("/api/Strategy/GetStrategyCases",
            {
                fromDate: fromDate,
                toDate: toDate
            }
           );
    }

    function GetBTSCases(fromDate, toDate) {
        return BaseAPIService.get("/api/Strategy/GetBTSCases",
            {
                fromDate: fromDate,
                toDate: toDate
            }
           );
    }

    function GetCellCases(fromDate, toDate) {
        return BaseAPIService.get("/api/Strategy/GetCellCases",
            {
                fromDate: fromDate,
                toDate: toDate
            }
           );
    }




   



});