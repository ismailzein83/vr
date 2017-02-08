(function (appControllers) {

    "use strict";

    PortalAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function PortalAccountAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "PortalAccount";

        function GetPortalAccountSettings(accountBEDefinitionId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetPortalAccountSettings'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            });
        }

        function AddPortalAccount(portalAccountEditorObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddPortalAccount'), portalAccountEditorObject);
        }

        function ResetPassword(resetPasswordInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'ResetPassword'), resetPasswordInput);
        }


        return ({
            GetPortalAccountSettings: GetPortalAccountSettings,
            AddPortalAccount: AddPortalAccount,
            ResetPassword: ResetPassword
        });
    }

    appControllers.service('Retail_BE_PortalAccountAPIService', PortalAccountAPIService);

})(appControllers);