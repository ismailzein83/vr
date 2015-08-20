
app.service('RawCDRLogAPIService', function (BaseAPIService) {

    return ({

        GetRawCDRData: GetRawCDRData,
    });


    function GetRawCDRData(query) {
        return BaseAPIService.post("/api/RawCDRLog/GetRawCDRData", query);
    }



});