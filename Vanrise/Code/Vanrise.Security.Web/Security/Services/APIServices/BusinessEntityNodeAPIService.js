(function (appControllers) {

    'use strict';

    BusinessEntityNodeAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService', 'SecurityService'];

    function BusinessEntityNodeAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'BusinessEntityNode';
        return {
            GetEntityNodes: GetEntityNodes,
            ToggleBreakInheritance: ToggleBreakInheritance,
            HasBreakInheritancePermission: HasBreakInheritancePermission
        };

        function GetEntityNodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetEntityNodes'));
        }

        function ToggleBreakInheritance(entityType, entityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'ToggleBreakInheritance'), {
                entityType: entityType,
                entityId: entityId
            });
        }

        function HasBreakInheritancePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['ToggleBreakInheritance']));
        }
    }

    appControllers.service('VR_Sec_BusinessEntityNodeAPIService', BusinessEntityNodeAPIService);

})(appControllers);