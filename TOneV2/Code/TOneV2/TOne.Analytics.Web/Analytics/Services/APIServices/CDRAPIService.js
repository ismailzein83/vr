
app.service('CDRAPIService', function (BaseAPIService) {

    return ({
       
        GetCDRData: GetCDRData
       
    });

 
    function GetCDRData(fromDate, toDate, nRecords, CDROption) {
        return BaseAPIService.get("/api/CDR/GetCDRData",
        {
            fromDate: fromDate,
            toDate: toDate,
            nRecords: nRecords,
            CDROption:CDROption
        }
        );
    }

});