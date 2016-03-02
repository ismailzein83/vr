(function (appControllers) {

    'use strict';

    AccountCaseHistoryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function AccountCaseHistoryAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {

        var controllerName = 'AccountCaseHistory';
       
        function GetFilteredAccountCaseHistoryByCaseID(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetFilteredAccountCaseHistoryByCaseID'), input);
        }

        return {
            GetFilteredAccountCaseHistoryByCaseID: GetFilteredAccountCaseHistoryByCaseID
        };

    }

    appControllers.service('CDRAnalysis_FA_AccountCaseHistoryAPIService', AccountCaseHistoryAPIService);

})(appControllers);