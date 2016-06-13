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

        return {
            GetFilteredChargingPolicies: GetFilteredChargingPolicies,
            GetChargingPolicy: GetChargingPolicy,
            AddChargingPolicy: AddChargingPolicy,
            UpdateChargingPolicy: UpdateChargingPolicy,
            HasAddChargingPolicyPermission: HasAddChargingPolicyPermission,
            HasUpdateChargingPolicyPermission: HasUpdateChargingPolicyPermission
        };
    }

    appControllers.service('Retail_BE_ChargingPolicyAPIService', ChargingPolicyAPIService);

})(appControllers);