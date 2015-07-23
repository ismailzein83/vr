
app.service('CDRAPIService', function (BaseAPIService) {

    return ({
       
        GetCDRData: GetCDRData,
        ExportCDRData: ExportCDRData
       
    });

 
    function GetCDRData(getCDRLogSummaryInput) {
        return BaseAPIService.post("/api/CDR/GetCDRData", getCDRLogSummaryInput);
    }
    function ExportCDRData(CDRLogSummaryInput) {
        return BaseAPIService.post("/api/CDR/ExportCDRData", CDRLogSummaryInput, {
            returnAllResponseParameters: true,
            responseTypeAsBufferArray: true
        });
    }

});