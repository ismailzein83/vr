(function (appControllers) {

    'use strict';

    VRAccountBalanceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function VRAccountBalanceAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'VRAccountBalance';

        function GetAccountBEDefinitionIdByAccountTypeId(accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountBEDefinitionIdByAccountTypeId"), {
                accountTypeId: accountTypeId
            });
        }
        return {
            GetAccountBEDefinitionIdByAccountTypeId: GetAccountBEDefinitionIdByAccountTypeId
        };
    }

    appControllers.service('Retail_BE_VRAccountBalanceAPIService', VRAccountBalanceAPIService);

})(appControllers);