(function (appControllers) {

    'use strict';

    AccountCaseHistoryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function AccountCaseHistoryAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {
        return {
            GetFilteredAccountCaseHistoryByCaseID: GetFilteredAccountCaseHistoryByCaseID
        };

        function GetFilteredAccountCaseHistoryByCaseID(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'AccountCaseHistory', 'GetFilteredAccountCaseHistoryByCaseID'), input);
        }
    }

    appControllers.service('CDRAnalysis_FA_AccountCaseHistoryAPIService', AccountCaseHistoryAPIService);

})(appControllers);