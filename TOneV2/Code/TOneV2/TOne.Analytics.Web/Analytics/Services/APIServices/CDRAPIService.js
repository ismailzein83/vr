
app.service('CDRAPIService', function (BaseAPIService) {

    return ({
       
        GetCDRData: GetCDRData
       
    });

 
    function GetCDRData(getCDRLogSummaryInput) {
        return BaseAPIService.post("/api/CDR/GetCDRData", getCDRLogSummaryInput);
    }

});