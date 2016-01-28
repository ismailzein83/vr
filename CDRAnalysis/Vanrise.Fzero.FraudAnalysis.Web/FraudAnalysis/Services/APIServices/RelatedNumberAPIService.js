(function (appControllers) {

    'use strict';

    RelatedNumberAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function RelatedNumberAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {
        return {
            GetRelatedNumbersByAccountNumber: GetRelatedNumbersByAccountNumber
        };

        function GetRelatedNumbersByAccountNumber(accountNumber) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'RelatedNumber', 'GetRelatedNumbersByAccountNumber'), {
                accountNumber: accountNumber
            });
        }
    }

    appControllers.service('CDRAnalysis_FA_RelatedNumberAPIService', RelatedNumberAPIService);

})(appControllers);