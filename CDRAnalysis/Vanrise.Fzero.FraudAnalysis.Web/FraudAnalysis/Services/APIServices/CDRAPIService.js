app.service('CDRAPIService', function (BaseAPIService) {

    return ({
        GetCDRs: GetCDRs
    });

    function GetCDRs(input) {
        return BaseAPIService.post("/api/CDR/GetCDRs", input );
    }


});