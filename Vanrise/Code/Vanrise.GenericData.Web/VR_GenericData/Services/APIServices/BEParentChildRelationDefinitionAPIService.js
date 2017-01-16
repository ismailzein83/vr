(function (appControllers) {

    'use strict';

    BEParentChildRelationDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig', 'SecurityService'];

    function BEParentChildRelationDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig, SecurityService) {

        var controllerName = "BEParentChildRelationDefinition";

        function GetBEParentChildRelationDefinitionsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetBEParentChildRelationDefinitionsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function GetBEParentChildRelationGridColumnNames(beParentChildRelationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetBEParentChildRelationGridColumnNames"), {
                beParentChildRelationDefinitionId: beParentChildRelationDefinitionId
            });
        }

        function GetBEParentChildRelationDefinition(beParentChildRelationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetBEParentChildRelationDefinition"), {
                beParentChildRelationDefinitionId: beParentChildRelationDefinitionId
            });
        }


        return ({
            GetBEParentChildRelationDefinitionsInfo: GetBEParentChildRelationDefinitionsInfo,
            GetBEParentChildRelationGridColumnNames: GetBEParentChildRelationGridColumnNames,
            GetBEParentChildRelationDefinition: GetBEParentChildRelationDefinition
        });
    }

    appControllers.service('VR_GenericData_BEParentChildRelationDefinitionAPIService', BEParentChildRelationDefinitionAPIService);

})(appControllers);