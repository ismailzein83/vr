(function (appControllers) {

    'use strict';

    DealProgressAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

    function DealProgressAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = 'DealProgress';

        function GetFilteredDealsProgress(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredDealsProgress'), input);
        }

    

        return ({
            GetFilteredDealsProgress: GetFilteredDealsProgress
        });
    }

    appControllers.service('WhS_BE_DealProgressAPIService', DealProgressAPIService);

})(appControllers);