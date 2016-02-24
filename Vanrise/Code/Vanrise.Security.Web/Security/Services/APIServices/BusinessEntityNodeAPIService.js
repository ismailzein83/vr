(function (appControllers) {

    'use strict';

    BusinessEntityNodeAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService', 'VR_Sec_SecurityAPIService'];

    function BusinessEntityNodeAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService, VR_Sec_SecurityAPIService) {
        return {
            GetEntityNodes: GetEntityNodes,
            ToggleBreakInheritance: ToggleBreakInheritance,
            HasBreakInheritancePermission: HasBreakInheritancePermission
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

        function HasBreakInheritancePermission() {
            return VR_Sec_SecurityAPIService.IsAllowed(VR_Sec_ModuleConfig.moduleName + '/BusinessEntityNode/ToggleBreakInheritance');
        }
    }

    appControllers.service('VR_Sec_BusinessEntityNodeAPIService', BusinessEntityNodeAPIService);

})(appControllers);