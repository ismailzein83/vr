app.service('CDRAPIService', function (BaseAPIService) {

    var controllerName = 'CDR';

    function GetCDRs(input) {
        return BaseAPIService.post(UtilsService.getServiceURL('api', controllerName, 'GetCDRs'), input);
    }

    return ({
        GetCDRs: GetCDRs
    });
});