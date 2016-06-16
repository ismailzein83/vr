(function (appControllers) {

    'use strict';

    MediationDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'Mediation_Generic_ModuleConfig'];

    function MediationDefinitionAPIService(BaseAPIService, UtilsService, SecurityService, Mediation_Generic_ModuleConfig) {
        var controllerName = "MediationDefinition";

        return ({
            GetFilteredMediationDefinitions: GetFilteredMediationDefinitions,
            GetMediationDefinition: GetMediationDefinition,
            AddMediationDefinition: AddMediationDefinition,
            HasAddMediationDefinition: HasAddMediationDefinition,
            UpdateMediationDefinition: UpdateMediationDefinition,
            HasUpdateMediationDefinition: HasUpdateMediationDefinition
        });

        function GetFilteredMediationDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Mediation_Generic_ModuleConfig.moduleName, controllerName, 'GetFilteredMediationDefinitions'), input);
        }

        function GetMediationDefinition(mediationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Mediation_Generic_ModuleConfig.moduleName, controllerName, 'GetMediationDefinition'), { mediationDefinitionId: mediationDefinitionId });
        }
        function AddMediationDefinition(mediationDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(Mediation_Generic_ModuleConfig.moduleName, 'MediationDefinition', 'AddMediationDefinition'), mediationDefinition);
        }
        function HasAddMediationDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Mediation_Generic_ModuleConfig.moduleName, controllerName, ['AddMediationDefinition']));
        }
        function UpdateMediationDefinition(mediationDefinitionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Mediation_Generic_ModuleConfig.moduleName, controllerName, 'UpdateMediationDefinition'), mediationDefinitionObject);
        }
        function HasUpdateMediationDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Mediation_Generic_ModuleConfig.moduleName, controllerName, ['UpdateMediationDefinition']));
        }
    }

    appControllers.service('Mediation_Generic_MediationDefinitionAPIService', MediationDefinitionAPIService);

})(appControllers);
