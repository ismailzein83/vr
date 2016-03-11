(function (appControllers) {

    "use strict";
    cdrAPIService.$inject = ['BaseAPIService', 'UtilsService'];

    function cdrAPIService(BaseAPIService, UtilsService) {

        var controllerName = 'CDR';

        function GetCDRs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL('api', controllerName, 'GetCDRs'), input);
        }

        return ({
            GetCDRs: GetCDRs
        });
    }

    appControllers.service('CDRAPIService', cdrAPIService);
})(appControllers);