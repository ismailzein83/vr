(function (appControllers) {

    'use strict';

    DIDAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_CustomerAccess_ModuleConfig', 'SecurityService'];

    function DIDAPIService(BaseAPIService, UtilsService, PartnerPortal_CustomerAccess_ModuleConfig, SecurityService) {
        var controllerName = 'DID';

        function GetFilteredDIDs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetFilteredDIDs"), input);
        };
        return {
            GetFilteredDIDs: GetFilteredDIDs,
        };
    }

    appControllers.service('PartnerPortal_CustomerAccess_DIDAPIService', DIDAPIService);

})(appControllers);