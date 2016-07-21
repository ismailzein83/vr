﻿(function (appControllers) {

    'use strict';

    LiveBalanceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function LiveBalanceAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'LiveBalance';

        function GetCurrentAccountBalance(accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetCurrentAccountBalance"), {
                accountId: accountId
            });
        }

        return {
            GetCurrentAccountBalance: GetCurrentAccountBalance
        };
    }

    appControllers.service('VR_AccountBalance_LiveBalanceAPIService', LiveBalanceAPIService);

})(appControllers);