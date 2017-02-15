(function (appControllers) {

    'use strict';

    VRRestAPIBEDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function VRRestAPIBEDefinitionAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {

        var controllerName = 'VRRestAPIBEDefinition';

        function GetRemoteBusinessEntityDefinitionsInfo(connectionId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetRemoteBusinessEntityDefinitionsInfo'), {
                connectionId: connectionId,
                serializedFilter: serializedFilter
            });
        }
         

        return {
            GetRemoteBusinessEntityDefinitionsInfo: GetRemoteBusinessEntityDefinitionsInfo
        };
    }

    appControllers.service('VR_GenericData_VRRestAPIBEDefinitionAPIService', VRRestAPIBEDefinitionAPIService);

})(appControllers);