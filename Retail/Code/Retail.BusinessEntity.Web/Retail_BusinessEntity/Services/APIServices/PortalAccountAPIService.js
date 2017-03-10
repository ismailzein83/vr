(function (appControllers) {

    "use strict";

    PortalAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function PortalAccountAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "PortalAccount";

        function GetPortalAccountSettings(accountBEDefinitionId, accountId, accountViewDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetPortalAccountSettings'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                accountViewDefinitionId: accountViewDefinitionId
            });
        }

        function AddPortalAccount(portalAccountEditorObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddPortalAccount'), portalAccountEditorObject);
        }

        function ResetPassword(resetPasswordInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'ResetPassword'), resetPasswordInput);
        }
        function DosesUserHaveConfigureAccess(accountBEDefinitionId, accountViewDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "DosesUserHaveConfigureAccess"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountViewDefinitionId: accountViewDefinitionId

            });
        }
        function DosesUserHaveResetPasswordAccess(accountBEDefinitionId, accountViewDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "DosesUserHaveResetPasswordAccess"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountViewDefinitionId: accountViewDefinitionId
            });
        }
        return ({
            GetPortalAccountSettings: GetPortalAccountSettings,
            AddPortalAccount: AddPortalAccount,
            ResetPassword: ResetPassword,
            DosesUserHaveConfigureAccess: DosesUserHaveConfigureAccess,
            DosesUserHaveResetPasswordAccess: DosesUserHaveResetPasswordAccess
        });
    }

    appControllers.service('Retail_BE_PortalAccountAPIService', PortalAccountAPIService);

})(appControllers);