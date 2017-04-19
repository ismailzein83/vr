(function (appControllers) {

    'use strict';

    AccountBalanceInvoicesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_InvToAccBalanceRelation_ModuleConfig', 'SecurityService'];

    function AccountBalanceInvoicesAPIService(BaseAPIService, UtilsService, VR_InvToAccBalanceRelation_ModuleConfig, SecurityService) {
        var controllerName = 'AccountBalanceInvoices';

        function GetFilteredAccountInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_InvToAccBalanceRelation_ModuleConfig.moduleName, controllerName, "GetFilteredAccountInvoices"), input);
        }
        return {
            GetFilteredAccountInvoices: GetFilteredAccountInvoices,
        };
    }

    appControllers.service('VR_InvToAccBalanceRelation_AccountBalanceInvoicesAPIService', AccountBalanceInvoicesAPIService);

})(appControllers);