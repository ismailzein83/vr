(function (appControllers) {

    'use strict';

    AccountCaseAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function AccountCaseAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {
        var controllerName = 'AccountCase';

        function GetLastAccountCase(accountNumber) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetLastAccountCase'), {
                accountNumber: accountNumber
            });
        }

        function GetFilteredAccountSuspicionSummaries(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetFilteredAccountSuspicionSummaries'), input);
        }

        function UpdateAccountCase(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'UpdateAccountCase'), input);
        }

        function HasUpdateAccountCasePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, ['UpdateAccountCase']));
        }

        function GetAccountCase(caseID) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetAccountCase'), {
                caseID: caseID
            });
        }

        function GetFilteredCasesByAccountNumber(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetFilteredCasesByAccountNumber'), input);
        }

        return {
            HasUpdateAccountCasePermission: HasUpdateAccountCasePermission,
            GetLastAccountCase: GetLastAccountCase,
            GetFilteredAccountSuspicionSummaries: GetFilteredAccountSuspicionSummaries,
            UpdateAccountCase: UpdateAccountCase,
            GetAccountCase: GetAccountCase,
            GetFilteredCasesByAccountNumber: GetFilteredCasesByAccountNumber
        };
    }

    appControllers.service('CDRAnalysis_FA_AccountCaseAPIService', AccountCaseAPIService);

})(appControllers);