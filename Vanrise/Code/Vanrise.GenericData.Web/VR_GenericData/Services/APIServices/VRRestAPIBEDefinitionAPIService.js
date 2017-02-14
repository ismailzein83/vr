(function (appControllers) {

    'use strict';

    BusinessEntityDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function BusinessEntityDefinitionAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {

        function GetBusinessEntityDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'BusinessEntityDefinition', 'GetBusinessEntityDefinitionsInfo'), {
                filter: filter
            });
        }


        return {
            GetBusinessEntityDefinitionsInfo: GetBusinessEntityDefinitionsInfo
        };
    }

    appControllers.service('VR_GenericData_BusinessEntityDefinitionAPIService', BusinessEntityDefinitionAPIService);

})(appControllers);