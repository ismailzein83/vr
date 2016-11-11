(function (appControllers) {

    'use strict';

    BalanceAccountTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function BalanceAccountTypeAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'BalanceAccountType';
        function GetBalanceAccountTypeInfos(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetBalanceAccountTypeInfos"));
        }
        return {
            GetBalanceAccountTypeInfos: GetBalanceAccountTypeInfos
        };
    }

    appControllers.service('VR_AccountBalance_BalanceAccountTypeAPIService', BalanceAccountTypeAPIService);

})(appControllers);