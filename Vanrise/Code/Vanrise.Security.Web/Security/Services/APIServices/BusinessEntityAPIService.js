(function (appControllers) {

    'use strict';

    BusinessEntityAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService', 'SecurityService'];

    function BusinessEntityAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'BusinessEntity';
        return {
            GetFilteredBusinessEntities: GetFilteredBusinessEntities,
            GetBusinessEntity: GetBusinessEntity,
            UpdateBusinessEntity: UpdateBusinessEntity,
            AddBusinessEntity: AddBusinessEntity
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

        function AddBusinessEntity(businessEntity) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'AddBusinessEntity'), businessEntity);
        }
    }

    appControllers.service('VR_Sec_BusinessEntityAPIService', BusinessEntityAPIService);

})(appControllers);