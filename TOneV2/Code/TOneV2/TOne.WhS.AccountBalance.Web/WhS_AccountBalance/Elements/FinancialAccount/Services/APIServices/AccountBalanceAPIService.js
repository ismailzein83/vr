(function (appControllers) {

    'use strict';

    AccountBalanceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_AccountBalance_ModuleConfig', 'SecurityService'];

    function AccountBalanceAPIService(BaseAPIService, UtilsService, WhS_AccountBalance_ModuleConfig, SecurityService) {

        var controllerName = 'AccountBalance';

        return {
        };
    }

    appControllers.service('WhS_AccountBalance_AccountBalanceAPIService', AccountBalanceAPIService);

})(appControllers);