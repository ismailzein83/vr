(function (appControllers) {

    'use strict';

    AccountBalanceTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountBalanceTypeAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'AccountBalance';

        function GetAccountBalanceTypeId(accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountBalanceTypeId"), {
                accountBEDefinitionId: accountBEDefinitionId
            });
        }
        return {
            GetAccountBalanceTypeId: GetAccountBalanceTypeId
        };
    }

    appControllers.service('Retail_BE_AccountBalanceTypeAPIService', AccountBalanceTypeAPIService);

})(appControllers);