(function (appControllers) {

    'use strict';

    RelatedNumberAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function RelatedNumberAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {

        var controllerName = 'RelatedNumber';

        function GetRelatedNumbersByAccountNumber(accountNumber) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetRelatedNumbersByAccountNumber'), {
                accountNumber: accountNumber
            });
        }

        return {
            GetRelatedNumbersByAccountNumber: GetRelatedNumbersByAccountNumber
        };
    }

    appControllers.service('CDRAnalysis_FA_RelatedNumberAPIService', RelatedNumberAPIService);

})(appControllers);