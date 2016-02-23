(function (appControllers) {

    'use strict';

    BusinessEntityNodeAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService'];

    function BusinessEntityNodeAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService) {
        return {
            GetEntityNodes: GetEntityNodes,
            ToggleBreakInheritance: ToggleBreakInheritance
        };

        function GetEntityNodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'BusinessEntityNode', 'GetEntityNodes'));
        }

        function ToggleBreakInheritance(entityType, entityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'BusinessEntityNode', 'ToggleBreakInheritance'), {
                entityType: entityType,
                entityId: entityId
            });
        }
    }

    appControllers.service('VR_Sec_BusinessEntityNodeAPIService', BusinessEntityNodeAPIService);

})(appControllers);