(function (appControllers) {

    'use strict';

    BillingTransactionTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function BillingTransactionTypeAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'BillingTransactionType';
        function GetBillingTransactionTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetBillingTransactionTypesInfo"));
        }
        return {
            GetBillingTransactionTypesInfo: GetBillingTransactionTypesInfo
        };
    }

    appControllers.service('VR_AccountBalance_BillingTransactionTypeAPIService', BillingTransactionTypeAPIService);

})(appControllers);