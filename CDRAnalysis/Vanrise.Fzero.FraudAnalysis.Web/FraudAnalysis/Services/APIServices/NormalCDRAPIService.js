app.service('NormalCDRAPIService', function (BaseAPIService) {

    return ({
        GetNormalCDRs: GetNormalCDRs
    });

    function GetNormalCDRs(input) {
        return BaseAPIService.post("/api/NormalCDR/GetNormalCDRs", input );
    }


});