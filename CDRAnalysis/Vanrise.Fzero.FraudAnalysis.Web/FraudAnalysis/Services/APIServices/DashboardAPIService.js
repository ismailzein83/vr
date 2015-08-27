app.service('DashboardAPIService', function (BaseAPIService) {

    return ({
        GetCasesSummary: GetCasesSummary,
        GetStrategyCases: GetStrategyCases,
        GetBTSCases: GetBTSCases,
        GetBTSHighValueCases: GetBTSHighValueCases,
        GetDailyVolumeLooses: GetDailyVolumeLooses
    });


    function GetCasesSummary(input) {
        return BaseAPIService.post("/api/Dashboard/GetCasesSummary",
            input
           );
    }

    function GetStrategyCases(fromDate, toDate) {
        return BaseAPIService.get("/api/Dashboard/GetStrategyCases",
            {
                fromDate: fromDate,
                toDate: toDate
            }
           );
    }

    function GetBTSCases(input) {
        return BaseAPIService.post("/api/Dashboard/GetBTSCases",
            input
           );
    }

    function GetBTSHighValueCases(input) {
        return BaseAPIService.post("/api/Dashboard/GetBTSHighValueCases",
            input
           );
    }


    function GetDailyVolumeLooses(input) {
        return BaseAPIService.post("/api/Dashboard/GetDailyVolumeLooses",
            input
           );
    }

   

});