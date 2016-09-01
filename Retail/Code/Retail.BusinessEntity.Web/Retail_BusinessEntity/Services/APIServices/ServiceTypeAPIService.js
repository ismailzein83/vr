(function (appControllers) {

    'use strict';

    ServiceTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function ServiceTypeAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'ServiceType';

        function GetFilteredServiceTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredServiceTypes'), input);
        }

        function GetServiceType(serviceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetServiceType'), {
                serviceTypeId: serviceTypeId
            });
        }

        function GetServiceTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetServiceTypesInfo'), {
                filter: filter
            });
        }

        function GetServiceTypeChargingPolicyDefinitionSettings(serviceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetServiceTypeChargingPolicyDefinitionSettings'), {
                serviceTypeId: serviceTypeId
            });
        }

        function UpdateServiceType(serviceType) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateServiceType'), serviceType);
        }

        function HasUpdateServiceTypePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateServiceType']));
        }
        return {
            GetFilteredServiceTypes: GetFilteredServiceTypes,
            GetServiceType: GetServiceType,
            GetServiceTypesInfo: GetServiceTypesInfo,
            GetServiceTypeChargingPolicyDefinitionSettings: GetServiceTypeChargingPolicyDefinitionSettings,
            UpdateServiceType: UpdateServiceType,
            HasUpdateServiceTypePermission: HasUpdateServiceTypePermission

        };
    }

    appControllers.service('Retail_BE_ServiceTypeAPIService', ServiceTypeAPIService);

})(appControllers);