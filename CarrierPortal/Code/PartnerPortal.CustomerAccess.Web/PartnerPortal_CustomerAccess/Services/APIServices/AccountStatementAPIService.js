(function (appControllers) {

    'use strict';

    AccountStatementAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_CustomerAccess_ModuleConfig', 'SecurityService'];

    function AccountStatementAPIService(BaseAPIService, UtilsService, PartnerPortal_CustomerAccess_ModuleConfig, SecurityService) {
        var controllerName = 'AccountStatement';
        function GetFilteredAccountStatments(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetFilteredAccountStatments"), input);
        }

        return {
            GetFilteredAccountStatments: GetFilteredAccountStatments,
        };
    }

    appControllers.service('PartnerPortal_CustomerAccess_AccountStatementAPIService', AccountStatementAPIService);

})(appControllers);