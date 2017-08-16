(function (appControllers) {

    'use strict';

    AccountStatementAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_CustomerAccess_ModuleConfig', 'SecurityService'];

    function AccountStatementAPIService(BaseAPIService, UtilsService, PartnerPortal_CustomerAccess_ModuleConfig, SecurityService) {
        var controllerName = 'AccountStatement';

        function GetFilteredAccountStatments(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetFilteredAccountStatments"), input);
        };

        function GetAccountStatementContextHandlerTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetAccountStatementContextHandlerTemplates"));
        };
        function GetAccountStatementExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetAccountStatementExtendedSettingsConfigs"));
        };
        function GetBalanceAccounts(viewId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetBalanceAccounts"), {
                viewId:viewId
            });
        };
        return {
            GetFilteredAccountStatments: GetFilteredAccountStatments,
            GetAccountStatementContextHandlerTemplates: GetAccountStatementContextHandlerTemplates,
            GetAccountStatementExtendedSettingsConfigs: GetAccountStatementExtendedSettingsConfigs,
            GetBalanceAccounts: GetBalanceAccounts

        };
    }

    appControllers.service('PartnerPortal_CustomerAccess_AccountStatementAPIService', AccountStatementAPIService);

})(appControllers);