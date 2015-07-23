
app.service('CDRAPIService', function (BaseAPIService) {

    return ({
       
        GetCDRData: GetCDRData,
        ExportCDRData: ExportCDRData
       
    });

 
    function GetCDRData(getCDRLogSummaryInput) {
        return BaseAPIService.post("/api/CDR/GetCDRData", getCDRLogSummaryInput);
    }
    function ExportCDRData(tempTableKey, nRecords) {
        return BaseAPIService.get("/api/CDR/ExportCDRData", {
            tempTableKey: tempTableKey,
            nRecords: nRecords
        }, {
            returnAllResponseParameters: true,
            responseTypeAsBufferArray: true
        });
    }

});