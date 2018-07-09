(function (appControllers) {

    'use strict';

    LKUPBusinessEntityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function LKUPBusinessEntityAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        var controllerName = 'LKUPBusinessEntity';

        return {
            GetLKUPBusinessEntityInfo: GetLKUPBusinessEntityInfo,
        };

        function GetLKUPBusinessEntityInfo(businessEntityDefinitionId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetLKUPBusinessEntityInfo'), {
                businessEntityDefinitionId: businessEntityDefinitionId,
                serializedFilter: serializedFilter
            });
        }
    }

    appControllers.service('VR_GenericData_LKUPBusinessEntityAPIService', LKUPBusinessEntityAPIService);

})(appControllers);