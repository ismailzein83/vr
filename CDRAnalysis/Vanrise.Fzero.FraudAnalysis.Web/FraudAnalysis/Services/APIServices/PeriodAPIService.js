(function (appControllers) {
    
    'use strict';

    PeriodAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function PeriodAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {
        return {
            GetPeriods: GetPeriods
        };

        function GetPeriods() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'Period', 'GetPeriods'));
        }
    }

    appControllers.service('CDRAnalysis_FA_PeriodAPIService', PeriodAPIService);

})(appControllers);