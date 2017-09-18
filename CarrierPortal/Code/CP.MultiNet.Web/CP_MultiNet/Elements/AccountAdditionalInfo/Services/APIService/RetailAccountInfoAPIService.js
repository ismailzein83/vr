(function (appControllers) {

    'use strict';

    AccountAdditionalInfoAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_MultiNet_ModuleConfig', 'SecurityService'];

    function AccountAdditionalInfoAPIService(BaseAPIService, UtilsService, CP_MultiNet_ModuleConfig, SecurityService) {
        var controllerName = 'AccountAdditionalInfo';

        function GetClientAccountAdditionalInfo(vrConnectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(CP_MultiNet_ModuleConfig.moduleName, controllerName, "GetClientAccountAdditionalInfo"), {
                vrConnectionId: vrConnectionId
            });
        };
        return {
            GetClientAccountAdditionalInfo: GetClientAccountAdditionalInfo,
        };
    }

    appControllers.service('CP_MultiNet_AccountAdditionalInfoAPIService', AccountAdditionalInfoAPIService);

})(appControllers);