
app.service('CDRAPIService', function (BaseAPIService) {

    return ({
       
        GetCDRData: GetCDRData
       
    });

 
    function GetCDRData(fromDate, toDate, nRecords) {
        return BaseAPIService.get("/api/CDR/GetCDRData",
        {
            fromDate: fromDate,
            toDate: toDate,
            nRecords:nRecords
        }
        );
    }

});