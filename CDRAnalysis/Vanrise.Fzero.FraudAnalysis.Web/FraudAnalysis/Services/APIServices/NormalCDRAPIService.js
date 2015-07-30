app.service('NormalCDRAPIService', function (BaseAPIService) {

    return ({
        GetNormalCDRs: GetNormalCDRs
    });

    function GetNormalCDRs(fromRow, toRow, fromDate, toDate, msisdn) {
        return BaseAPIService.get("/api/NormalCDR/GetNormalCDRs",
            {
                fromRow: fromRow,
                toRow: toRow,
                fromDate: fromDate,
                toDate: toDate,
                msisdn: msisdn
            }
           );
    }


});