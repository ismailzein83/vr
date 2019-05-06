(function (appControllers) {

    'use strict';

    PeriodDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_RA_ModuleConfig'];

    function PeriodDefinitionAPIService(BaseAPIService, UtilsService, Retail_RA_ModuleConfig) {
        var controllerName = 'RAPeriodDefinitionController';

        function GetPeriodDefinitionInfo(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_RA_ModuleConfig.moduleName, controllerName, 'GetPeriodDefinitionInfo'),input);
        }
        return {
            GetPeriodDefinitionInfo: GetPeriodDefinitionInfo
        };
    }

    appControllers.service('Retail_RA_PeriodDefinitionAPIService', PeriodDefinitionAPIService);

})(appControllers);