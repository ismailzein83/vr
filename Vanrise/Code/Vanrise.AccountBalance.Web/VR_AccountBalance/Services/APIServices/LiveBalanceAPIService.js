(function (appControllers) {

    'use strict';

    LiveBalanceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function LiveBalanceAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'LiveBalance';

        function GetCurrentAccountBalance(accountId, accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetCurrentAccountBalance"), {
                accountTypeId: accountTypeId,
                accountId: accountId
            });
        }
        function GetFilteredAccountBalances(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetFilteredAccountBalances"), input);
        }
        return {
            GetCurrentAccountBalance: GetCurrentAccountBalance,
            GetFilteredAccountBalances: GetFilteredAccountBalances
        };
    }

    appControllers.service('VR_AccountBalance_LiveBalanceAPIService', LiveBalanceAPIService);

})(appControllers);