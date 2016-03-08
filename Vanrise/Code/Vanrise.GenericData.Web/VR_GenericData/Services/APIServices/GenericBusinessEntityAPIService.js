 (function (appControllers) {

    'use strict';

    GenericBusinessEntityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericBusinessEntityAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        var controllerName = 'GenericBusinessEntity';

        return {
            GetFilteredGenericBusinessEntities: GetFilteredGenericBusinessEntities,
            GetGenericBusinessEntity: GetGenericBusinessEntity,
            AddGenericBusinessEntity: AddGenericBusinessEntity,
            UpdateGenericBusinessEntity: UpdateGenericBusinessEntity,
            GetGenericBusinessEntityInfo: GetGenericBusinessEntityInfo,
            GetBusinessEntityTitle: GetBusinessEntityTitle,
            DeleteGenericBusinessEntity: DeleteGenericBusinessEntity
        };

        function GetFilteredGenericBusinessEntities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetFilteredGenericBusinessEntities'), input);
        }

        function GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetGenericBusinessEntity'), {
                genericBusinessEntityId: genericBusinessEntityId,
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }
        function GetGenericBusinessEntityInfo(businessEntityDefinitionId,serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetGenericBusinessEntityInfo'), {
                businessEntityDefinitionId:businessEntityDefinitionId,
                serializedFilter: serializedFilter
            });
        }
        function AddGenericBusinessEntity(genericBusinessEntity) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'AddGenericBusinessEntity'), genericBusinessEntity);
        }

        function UpdateGenericBusinessEntity(genericBusinessEntity) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'UpdateGenericBusinessEntity'), genericBusinessEntity);
        }

        function GetBusinessEntityTitle(businessEntityDefinitionId, genericBussinessEntityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetBusinessEntityTitle'), {
                businessEntityDefinitionId: businessEntityDefinitionId,
                genericBussinessEntityId: genericBussinessEntityId
            });
        }

        function DeleteGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'DeleteGenericBusinessEntity'), {
                genericBusinessEntityId: genericBusinessEntityId,
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }
    }

    appControllers.service('VR_GenericData_GenericBusinessEntityAPIService', GenericBusinessEntityAPIService);

})(appControllers);