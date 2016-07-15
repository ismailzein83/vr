(function (appControllers) {

    'use strict';

    BusinessEntityAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService', 'SecurityService'];

    function BusinessEntityAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'BusinessEntity';
        return {
            GetFilteredBusinessEntities: GetFilteredBusinessEntities,
            GetBusinessEntity: GetBusinessEntity,
            UpdateBusinessEntity: UpdateBusinessEntity,
            AddBusinessEntity: AddBusinessEntity,
            HasUpdateBusinessEntityPermission: HasUpdateBusinessEntityPermission,
            HasAddBusinessEntityPermission: HasAddBusinessEntityPermission,
            GetBusinessEntitiesByIds: GetBusinessEntitiesByIds
        };

        function GetFilteredBusinessEntities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetFilteredBusinessEntities'), input);
        }

        function GetBusinessEntity(entityId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetBusinessEntity'),
                {
                    entityId: entityId
                });
        }

        function UpdateBusinessEntity(businessEntity) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateBusinessEntity'), businessEntity);
        }
        function GetBusinessEntitiesByIds(entitiesIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetBusinessEntitiesByIds'), entitiesIds);
        }
        function HasUpdateBusinessEntityPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateBusinessEntity']));
        }
        function AddBusinessEntity(businessEntity) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'AddBusinessEntity'), businessEntity);
        }


        function HasAddBusinessEntityPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['AddBusinessEntity']));
        }
    }

    appControllers.service('VR_Sec_BusinessEntityAPIService', BusinessEntityAPIService);

})(appControllers);