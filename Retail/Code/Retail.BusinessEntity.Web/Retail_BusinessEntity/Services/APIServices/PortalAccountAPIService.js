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

        function GetPortalAccountDetails(accountBEDefinitionId, accountId, accountViewDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetPortalAccountDetails'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                accountViewDefinitionId: accountViewDefinitionId
            });

        }
        function GetPortalAccount(accountBEDefinitionId, accountId, accountViewDefinitionId, userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetPortalAccount'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                accountViewDefinitionId: accountViewDefinitionId,
                userId: userId
            });

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
        function AddAdditionalPortalAccount(portalAccountEditorObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddAdditionalPortalAccount'), portalAccountEditorObject);
        }

        function UpdatePortalAccount(portalAccountEditorObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdatePortalAccount'), portalAccountEditorObject);
        }
        function EnbalePortalAccount(accountBEDefinitionId, accountViewDefinitionId, accountId, userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'EnbalePortalAccount'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountViewDefinitionId: accountViewDefinitionId,
                userId: userId,
                accountId: accountId
            });
        }
      
        function UnlockPortalAccount(accountBEDefinitionId, accountViewDefinitionId, accountId, userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UnlockPortalAccount'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountViewDefinitionId: accountViewDefinitionId,
                userId: userId,
                accountId: accountId
            });
        }
        function DisablePortalAccount(accountBEDefinitionId, accountViewDefinitionId, accountId, userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'DisablePortalAccount'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountViewDefinitionId: accountViewDefinitionId,
                userId: userId,
                accountId: accountId

            });
        }



        return ({
            GetPortalAccountSettings: GetPortalAccountSettings,
            AddPortalAccount: AddPortalAccount,
            ResetPassword: ResetPassword,
            DosesUserHaveConfigureAccess: DosesUserHaveConfigureAccess,
            DosesUserHaveResetPasswordAccess: DosesUserHaveResetPasswordAccess,
            AddAdditionalPortalAccount: AddAdditionalPortalAccount,
            UpdatePortalAccount: UpdatePortalAccount,
            GetPortalAccountDetails: GetPortalAccountDetails,
            GetPortalAccount: GetPortalAccount,
            EnbalePortalAccount: EnbalePortalAccount,
            DisablePortalAccount: DisablePortalAccount,
            UnlockPortalAccount: UnlockPortalAccount
        });
    }

    appControllers.service('Retail_BE_PortalAccountAPIService', PortalAccountAPIService);

})(appControllers);