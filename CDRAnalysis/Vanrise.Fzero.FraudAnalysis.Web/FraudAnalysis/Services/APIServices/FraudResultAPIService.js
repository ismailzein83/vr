app.service('FraudResultAPIService', function (BaseAPIService) {

    return ({
        GetFilteredSuspiciousNumbers: GetFilteredSuspiciousNumbers
    });

    function GetFilteredSuspiciousNumbers(fromRow, toRow, fromDate, toDate, strategyId, suspicionLevelsList) {
        return BaseAPIService.get("/api/Strategy/GetFilteredSuspiciousNumbers",
            {
                fromRow: fromRow,
                toRow: toRow,
                fromDate: fromDate,
                toDate: toDate,
                strategyId: strategyId,
                suspicionLevelsList: suspicionLevelsList,
            }
           );
    }


   




});