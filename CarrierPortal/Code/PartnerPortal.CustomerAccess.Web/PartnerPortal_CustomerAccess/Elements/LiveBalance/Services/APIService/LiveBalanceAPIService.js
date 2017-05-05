(function (appControllers) {

    'use strict';

    LiveBalanceAPIServiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_CustomerAccess_ModuleConfig', 'SecurityService'];

    function LiveBalanceAPIServiceAPIService(BaseAPIService, UtilsService, PartnerPortal_CustomerAccess_ModuleConfig, SecurityService) {
        var controllerName = 'LiveBalance';

        function GetCurrentAccountBalance(connectionId, accountTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetCurrentAccountBalance"), {
                accountTypeId: accountTypeId,
                connectionId: connectionId,
            });
        };
        return {
            GetCurrentAccountBalance: GetCurrentAccountBalance,
        };
    }

    appControllers.service('PartnerPortal_CustomerAccess_LiveBalanceAPIService', LiveBalanceAPIServiceAPIService);

})(appControllers);