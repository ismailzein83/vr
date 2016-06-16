(function (appControllers) {

    'use strict';

    ChargingPolicyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function ChargingPolicyAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService)
    {
        var controllerName = 'ChargingPolicy';

        function GetFilteredChargingPolicies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredChargingPolicies'), input);
        }

        function GetChargingPolicy(chargingPolicyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetChargingPolicy'), {
                chargingPolicyId: chargingPolicyId
            });
        }
        function GetChargingPoliciesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetChargingPoliciesInfo'), {
                filter: filter
            });
        }

        
        function AddChargingPolicy(chargingPolicy) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddChargingPolicy'), chargingPolicy);
        }

        function UpdateChargingPolicy(chargingPolicy) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateChargingPolicy'), chargingPolicy);
        }

        function HasAddChargingPolicyPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddChargingPolicy']));
        }

        function HasUpdateChargingPolicyPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateChargingPolicy']));
        }
        function GetChargingPolicyTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetChargingPolicyTemplateConfigs'));
        }

        function GetChargingPolicyPartTypeTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetChargingPolicyPartTypeTemplateConfigs'));
        }
        function GetChargingPolicyPartTemplateConfigs(partTypeConfigId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetChargingPolicyPartTemplateConfigs'), { partTypeConfigId: partTypeConfigId });
        }

        return {
            GetFilteredChargingPolicies: GetFilteredChargingPolicies,
            GetChargingPoliciesInfo:GetChargingPoliciesInfo,
            GetChargingPolicy: GetChargingPolicy,
            AddChargingPolicy: AddChargingPolicy,
            UpdateChargingPolicy: UpdateChargingPolicy,
            HasAddChargingPolicyPermission: HasAddChargingPolicyPermission,
            HasUpdateChargingPolicyPermission: HasUpdateChargingPolicyPermission,
            GetChargingPolicyTemplateConfigs: GetChargingPolicyTemplateConfigs,
            GetChargingPolicyPartTypeTemplateConfigs: GetChargingPolicyPartTypeTemplateConfigs,
            GetChargingPolicyPartTemplateConfigs: GetChargingPolicyPartTemplateConfigs
        };
    }

    appControllers.service('Retail_BE_ChargingPolicyAPIService', ChargingPolicyAPIService);

})(appControllers);