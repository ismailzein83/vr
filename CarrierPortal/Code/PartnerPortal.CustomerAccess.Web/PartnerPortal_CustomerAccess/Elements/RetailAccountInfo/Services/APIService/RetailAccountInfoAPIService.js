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
        return {
            GetClientProfileAccountInfo: GetClientProfileAccountInfo,
        };
    }

    appControllers.service('PartnerPortal_CustomerAccess_RetailAccountInfoAPIService', RetailAccountInfoAPIService);

})(appControllers);