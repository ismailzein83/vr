(function (appControllers) {

    'use strict';

    RelationDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_InvToAccBalanceRelation_ModuleConfig', 'SecurityService'];

    function RelationDefinitionAPIService(BaseAPIService, UtilsService, VR_InvToAccBalanceRelation_ModuleConfig, SecurityService) {
        var controllerName = 'RelationDefinition';

        function GetRelationDefinitionExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_InvToAccBalanceRelation_ModuleConfig.moduleName, controllerName, "GetRelationDefinitionExtendedSettingsConfigs"));
        }

        return {
            GetRelationDefinitionExtendedSettingsConfigs: GetRelationDefinitionExtendedSettingsConfigs,
        };
    }

    appControllers.service('VR_InvToAccBalanceRelation_RelationDefinitionAPIService', RelationDefinitionAPIService);

})(appControllers);