(function (appControllers) {

    'use strict';

    AccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig'];

    function AccountAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig) {
        var controllerName = 'Account';

        function GetAccountInfo(accountTypeId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountInfo"), {
                accountTypeId: accountTypeId,
                accountId: accountId
            });
        }

        return {
            GetAccountInfo: GetAccountInfo,
        };
    }

    appControllers.service('VR_AccountBalance_AccountAPIService', AccountAPIService);

})(appControllers);