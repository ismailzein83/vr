app.service('FraudResultAPIService', function (BaseAPIService) {

    return ({
        GetFilteredSuspiciousNumbers: GetFilteredSuspiciousNumbers
    });

    function GetFilteredSuspiciousNumbers( fromRow,  toRow,  fromDate,  toDate,  strategyId,  suspicionList) {
        return BaseAPIService.get("/api/FraudResult/GetFilteredSuspiciousNumbers",
            {
                fromRow: fromRow,
                toRow: toRow,
                fromDate: fromDate,
                toDate: toDate,
                strategyId: strategyId,
                suspicionList: suspicionList,
            }
           );
    }


   




});