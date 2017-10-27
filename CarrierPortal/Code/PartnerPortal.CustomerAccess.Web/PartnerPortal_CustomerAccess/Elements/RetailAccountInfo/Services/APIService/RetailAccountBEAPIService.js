(function (appControllers) {

    'use strict';

    RetailAccountBEAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_CustomerAccess_ModuleConfig', 'SecurityService'];

    function RetailAccountBEAPIService(BaseAPIService, UtilsService, PartnerPortal_CustomerAccess_ModuleConfig, SecurityService) {
        var controllerName = 'RetailAccountBE';

        function GetClientChildAccountsInfo(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetClientChildAccountsInfo"), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        };
        return {
            GetClientChildAccountsInfo: GetClientChildAccountsInfo,
        };
    }

    appControllers.service('PartnerPortal_CustomerAccess_RetailAccountBEAPIService', RetailAccountBEAPIService);

})(appControllers);