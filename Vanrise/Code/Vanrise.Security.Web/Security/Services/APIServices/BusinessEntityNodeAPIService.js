(function (appControllers) {

    'use strict';

    BusinessEntityNodeAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService', 'SecurityService'];

    function BusinessEntityNodeAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'BusinessEntityNode';
        return {
            GetEntityNodes: GetEntityNodes,
            ToggleBreakInheritance: ToggleBreakInheritance,
            HasBreakInheritancePermission: HasBreakInheritancePermission,
            GetEntityModules: GetEntityModules,
            UpdateEntityNodesRank: UpdateEntityNodesRank,
            HasUpdateEntityNodesRankPermission: HasUpdateEntityNodesRankPermission
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
        function GetEntityModules() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetEntityModules'));
        }
        function UpdateEntityNodesRank(businessEntityNodes) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateEntityNodesRank'), businessEntityNodes);
        }
        function HasUpdateEntityNodesRankPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateEntityNodesRank']));
        }
    }

    appControllers.service('VR_Sec_BusinessEntityNodeAPIService', BusinessEntityNodeAPIService);

})(appControllers);