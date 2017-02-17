(function (appControllers) {

    'use strict';

    VRRestAPIBusinessEntityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function VRRestAPIBusinessEntityAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {

        var controllerName = "VRRestAPIBusinessEntity";

        function GetBusinessEntitiesInfo(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetBusinessEntitiesInfo'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }

        return {
            GetBusinessEntitiesInfo: GetBusinessEntitiesInfo
        };
    }

    appControllers.service('VR_GenericData_VRRestAPIBusinessEntityAPIService', VRRestAPIBusinessEntityAPIService);

})(appControllers);