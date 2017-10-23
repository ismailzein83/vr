(function (appControllers) {

    'use strict';

    RetailAccountUserAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_CustomerAccess_ModuleConfig', 'SecurityService'];

    function RetailAccountUserAPIService(BaseAPIService, UtilsService, PartnerPortal_CustomerAccess_ModuleConfig, SecurityService) {
        var controllerName = 'RetailAccountUser';

        function GetClientChildAccountsInfo(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetClientChildAccountsInfo"), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        };
        return {
            GetClientChildAccountsInfo: GetClientChildAccountsInfo,
        };
    }

    appControllers.service('PartnerPortal_CustomerAccess_RetailAccountUserAPIService', RetailAccountUserAPIService);

})(appControllers);