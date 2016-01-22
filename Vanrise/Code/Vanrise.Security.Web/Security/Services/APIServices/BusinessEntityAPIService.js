(function (appControllers) {

    'use strict';

    BusinessEntityAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService'];

    function BusinessEntityAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService) {
        return ({
            GetEntityNodes: GetEntityNodes,
            ToggleBreakInheritance: ToggleBreakInheritance
        });

        function GetEntityNodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'BusinessEntity', 'GetEntityNodes'));
        }

        function ToggleBreakInheritance(entityType, entityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'BusinessEntity', 'ToggleBreakInheritance'), {
                entityType: entityType,
                entityId: entityId
            });
        }
    }

    appControllers.service('VR_Sec_BusinessEntityAPIService', BusinessEntityAPIService);

})(appControllers);
