(function (appControllers) {

    "use strict";

    PortalAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function PortalAccountAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "PortalAccount";

        function AddPortalAccount(portalAccountEditorObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddPortalAccount'), portalAccountEditorObject);
        }

        function GetPortalUserAccount(accountBEDefinitionId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetPortalUserAccount'), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            });
        }


        return ({
            AddPortalAccount: AddPortalAccount,
            GetPortalUserAccount: GetPortalUserAccount
        });
    }

    appControllers.service('Retail_BE_PortalAccountAPIService', PortalAccountAPIService);

})(appControllers);