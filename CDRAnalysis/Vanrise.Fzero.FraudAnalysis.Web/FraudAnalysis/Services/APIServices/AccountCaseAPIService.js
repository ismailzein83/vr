﻿(function (appControllers) {

    'use strict';

    AccountCaseAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function AccountCaseAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {
        return {
            GetLastAccountCase: GetLastAccountCase,
            GetFilteredAccountSuspicionSummaries: GetFilteredAccountSuspicionSummaries,
            UpdateAccountCase: UpdateAccountCase,
            GetAccountCase: GetAccountCase,
            GetFilteredCasesByAccountNumber: GetFilteredCasesByAccountNumber
        };

        function GetLastAccountCase(accountNumber) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'AccountCase', 'GetLastAccountCase'), {
                accountNumber: accountNumber
            });
        }

        function GetFilteredAccountSuspicionSummaries(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'AccountCase', 'GetFilteredAccountSuspicionSummaries'), input);
        }

        function UpdateAccountCase(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'AccountCase', 'UpdateAccountCase'), input);
        }
        function GetAccountCase(caseID) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'AccountCase', 'GetAccountCase'), {
                caseID: caseID
            });
        }
        function GetFilteredCasesByAccountNumber(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, 'AccountCase', 'GetFilteredCasesByAccountNumber'), input);
        }
    }

    appControllers.service('CDRAnalysis_FA_AccountCaseAPIService', AccountCaseAPIService);

})(appControllers);