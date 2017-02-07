(function (appControllers) {

    "use strict";

    PortalAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function PortalAccountAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "PortalAccount";

        function AddPortalAccount(portalAccountEditorObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddPortalAccount'), portalAccountEditorObject);
        }

        function GetPortalAccountSettings(accountBEDefinitionId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetPortalAccountSettings'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            });
        }


        return ({
            AddPortalAccount: AddPortalAccount,
            GetPortalAccountSettings: GetPortalAccountSettings
        });
    }

    appControllers.service('Retail_BE_PortalAccountAPIService', PortalAccountAPIService);

})(appControllers);