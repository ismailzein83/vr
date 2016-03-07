 (function (appControllers) {

    'use strict';

    GenericBusinessEntityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericBusinessEntityAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        var controllerName = 'GenericBusinessEntity';

        return {
            GetFilteredGenericBusinessEntities: GetFilteredGenericBusinessEntities,
            GetGenericBusinessEntity: GetGenericBusinessEntity,
            AddGenericBusinessEntity: AddGenericBusinessEntity,
            UpdateExtensibleBEItem: UpdateExtensibleBEItem
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
        
        function AddGenericBusinessEntity(genericBusinessEntity) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'AddGenericBusinessEntity'), genericBusinessEntity);
        }

        function UpdateExtensibleBEItem(genericBusinessEntity) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'UpdateExtensibleBEItem'), genericBusinessEntity);
        }
    }

    appControllers.service('VR_GenericData_GenericBusinessEntityAPIService', GenericBusinessEntityAPIService);

})(appControllers);