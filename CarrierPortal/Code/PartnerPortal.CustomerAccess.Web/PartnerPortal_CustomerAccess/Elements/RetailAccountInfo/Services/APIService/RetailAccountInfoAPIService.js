(function (appControllers) {

    'use strict';

    RetailAccountInfoAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_CustomerAccess_ModuleConfig', 'SecurityService'];

    function RetailAccountInfoAPIService(BaseAPIService, UtilsService, PartnerPortal_CustomerAccess_ModuleConfig, SecurityService) {
        var controllerName = 'RetailAccountInfo';

        function GetClientProfileAccountInfo(vrConnectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetClientProfileAccountInfo"), {
                vrConnectionId: vrConnectionId
            });
        };
        function GetRemoteGenericFieldDefinitionsInfo(vrConnectionId, accountBEDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, 'GetRemoteGenericFieldDefinitionsInfo'), {
                vrConnectionId: vrConnectionId,
                accountBEDefinitionId: accountBEDefinitionId
            });
        }
        function GetSubAccountsGridColumnAttributes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, 'GetSubAccountsGridColumnAttributes'), input);
        }
        function GetFilteredSubAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, 'GetFilteredSubAccounts'), input);
        }
        return {
            GetClientProfileAccountInfo: GetClientProfileAccountInfo,
            GetRemoteGenericFieldDefinitionsInfo: GetRemoteGenericFieldDefinitionsInfo,
            GetSubAccountsGridColumnAttributes: GetSubAccountsGridColumnAttributes,
            GetFilteredSubAccounts: GetFilteredSubAccounts
        };
    }

    appControllers.service('PartnerPortal_CustomerAccess_RetailAccountInfoAPIService', RetailAccountInfoAPIService);

})(appControllers);